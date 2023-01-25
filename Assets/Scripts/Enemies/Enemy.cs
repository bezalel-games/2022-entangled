using System;
using Effects;
using HP_System;
using Managers;
using Player;
using Rooms;
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
        private static readonly int DeadAnimationID = Animator.StringToHash("Dead");
        private static readonly int MoveAnimationID = Animator.StringToHash("Move");

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

        public int SplitCount { get; set; }
        public int Index { get; set; }

        #endregion

        #region Events

        public event Action Enabled;

        #endregion

        #region Function Events

        protected override void Awake()
        {
            base.Awake();
            _roomEnemies = transform.parent != null ? transform.parent.GetComponent<RoomEnemies>() : null;
            _collider = GetComponent<Collider2D>();
            Layer = LayerMask.GetMask("Enemies");
        }

        protected override void OnEnable()
        {
            Frozen = true;
            
            if (RoomManager.IsTutorial)
            {
                Enabled?.Invoke();
                Invulnerable = true;
                DelayInvoke((() =>
                {
                    Frozen = false;
                    Invulnerable = false;
                }), 0.5f);
            }
            
            base.OnEnable();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            Move();
        }

        protected virtual void OnCollisionEnter2D(Collision2D other)
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

                if (line == null || other is not PolygonCollider2D) return;
                if (HasBarrier)
                {
                    Barrier.Active = false;
                    return;
                }

                Stun(line.EnemyFreezeTime);
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
            if (SplitCount > 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    var split = RoomManager.EnemyDictionary[Index].Spawn(transform.position, transform.parent, true)
                        .enemy;
                    split.Enabled?.Invoke();
                    split.SplitCount = SplitCount - 1;
                }

                _roomEnemies.AddLivingCount(2);
                AfterDeathAnimation();
                return;
            }
            
            Animator.SetTrigger(DeadAnimationID);
        }

        public void AfterDeathAnimation()
        {
            if (_attacking)
                NumberOfAttacking--;
            
            gameObject.SetActive(false);
            _roomEnemies.EnemyKilled();
        }

        public override void Stop()
        {
            base.Stop();
            Animator.SetBool(MoveAnimationID,false);
            DesiredDirection = Vector2.zero;
        }

        public override void Stun(float duration)
        {
            GameManager.PlayEffect(transform.position, Effect.EffectType.STUN);
            base.Stun(duration);
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

        protected override void HitShake() => CameraManager.EnemyHitShake();
        
        #endregion
    }
}