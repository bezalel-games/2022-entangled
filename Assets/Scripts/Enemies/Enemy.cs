using System;
using HP_System;
using Player;
using UnityEngine;

namespace Enemies
{
    public class Enemy : LivingBehaviour
    {
        #region Serialized Fields

        [Header("Enemy")] 
        [SerializeField] protected float _damage;

        #endregion

        #region Non-Serialized Fields

        private static int _numAttacking;
        private static int _maxAttacking = 2;

        private RoomEnemies _roomEnemies;
        private Vector2 _desiredDirection;
        private bool _canAttack = true;
        private bool _attacking;

        private Collider2D _collider;

        private bool _canTrailDamage = true;

        #endregion

        #region Properties

        [field: SerializeField] public float MaxSpeed { get; set; }
        [field: SerializeField] public float AttackCooldown { get; set; }
        [field: SerializeField] public float MpRestore { get; set; }
        [field: SerializeField] public float AttackDistance { get; private set; }
        [field: SerializeField] public float KeepDistance { get; private set; }

        [field: SerializeField] public float PrepareAttackTime { get; private set; }

        [field: SerializeField] public float AttackTime { get; private set; } = 1;

        [field: SerializeField] public int Rank { get; set; } = 1;

        [field: SerializeField] public float SeparationWeight { get; set; } = 1;
        [field: SerializeField] public float ToPlayerWeight { get; set; } = 1;
        [field: SerializeField] public float AwayFromWallWeight { get; set; } = 1;

        public Vector2 DesiredDirection
        {
            get => _desiredDirection;
            set
            {
                _desiredDirection = value.normalized;
                var xVal = Mathf.Abs(_desiredDirection.x) <= 0.1f ? 0 : _desiredDirection.x;
                var yVal = Mathf.Abs(_desiredDirection.y) <= 0.1f ? 0 : _desiredDirection.y;
                if (xVal == 0)
                {
                    Renderer.flipX = false;
                }
                else
                {
                    Renderer.flipX = xVal > 0;
                }

                Animator.SetFloat(xDirection, xVal);
                Animator.SetFloat(yDirection, yVal);
            }
        }

        public bool CanAttack
        {
            get => _canAttack && NumberOfAttacking < MaxCanAttack && !Frozen;
            set => _canAttack = value;
        }

        public static LayerMask Layer { get; private set; }

        public static int NumberOfAttacking
        {
            get => _numAttacking;
            set => _numAttacking = Math.Max(Math.Min(value, MaxCanAttack), 0);
        }

        public static int MaxCanAttack
        {
            get => _maxAttacking;
            set => _maxAttacking = Math.Max(_maxAttacking, 0);
        }

        public bool Attacking
        {
            get => _attacking;
            set
            {
                _attacking = value;
                NumberOfAttacking += value ? 1 : -1;
            }
        }

        #endregion

        #region Events

        public event Action Enabled;

        #endregion

        #region Function Events

        protected override void Awake()
        {
            base.Awake();
            Frozen = true;
            _roomEnemies = transform.parent != null ? transform.parent.GetComponent<RoomEnemies>() : null;
            _collider = GetComponent<Collider2D>();
            Layer = LayerMask.GetMask("Enemies");
        }

        protected override void OnEnable()
        {
            Enabled?.Invoke();
            base.OnEnable();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            Move();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                var hittable = other.gameObject.GetComponent<IHittable>();
                if (hittable == null) return;

                hittable.OnHit(transform, _damage);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Precision"))
            {
                Line line = other.GetComponent<Line>();
                
                if(line == null) return;

                if (other is PolygonCollider2D)
                {
                    if (HasBarrier)
                    {
                        ToggleBarrier(false);
                        return;
                    }
                    Stop();
                    Renderer.color = Color.black;
                    Frozen = true;
                    DelayInvoke((() => 
                    {
                        Renderer.color = Color.white;
                        Frozen = false;
                    }), line.EnemyFreezeTime);
                }
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Precision"))
            {
                Line line = other.GetComponent<Line>();

                if (line == null) return;
                
                if (other is EdgeCollider2D)
                {
                    if (!_canTrailDamage) return;

                    _canTrailDamage = false;
                    OnHit(line.transform, line.Damage, false);
                    DelayInvoke((() => { _canTrailDamage = true; }), line.DamageCooldown);
                }
            }
        }

        #endregion

        #region Public Methods

        public override void OnDie()
        {
            _collider.enabled = false;
            Stop();
            Animator.SetTrigger("Dead");
        }

        public void AfterDeathAnimation()
        {
            if (_attacking)
                NumberOfAttacking--;
            
            gameObject.SetActive(false);
            _roomEnemies.EnemyKilled();
        }

        public void Stop()
        {
            Animator.SetBool("Move",false);
            DesiredDirection = Vector2.zero;
            Rigidbody.velocity = Vector2.zero;
        }

        #endregion

        #region Private Methods

        protected virtual void Move()
        {
            if (Rigidbody.bodyType == RigidbodyType2D.Static || IsDead || Frozen)
                return;

            if (PushbackVector != Vector2.zero)
            {
                Rigidbody.velocity = PushbackVector;
            }
            else
            {
                Rigidbody.velocity = _desiredDirection * MaxSpeed;
            }
        }

        #endregion
    }
}