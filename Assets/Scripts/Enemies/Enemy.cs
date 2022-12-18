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
        [SerializeField] private float _speed;
        [SerializeField] protected float _damage;

        #endregion

        #region Non-Serialized Fields

        private RoomEnemies _roomEnemies;
        protected Rigidbody2D _rigidbody;
        private Vector2 _desiredDirection;
        private bool _canAttack = true;

        #endregion

        #region Properties
        
        [field: SerializeField] public float AttackCooldown { get; set; }
        [field: SerializeField] public float MpRestore { get; set; }
        [field: SerializeField] public float AttackDistance { get; private set; }
        [field: SerializeField] public float KeepDistance { get; private set; }

        [field: SerializeField] public float PrepareAttackTime { get; private set; }

        [field: SerializeField] public int Rank { get; set; } = 1;
        
        [field: SerializeField] public float SeparationWeight { get; set; } = 1;
        [field: SerializeField] public float ToPlayerWeight { get; set; } = 1;

        public Vector2 DesiredDirection
        {
            get => _desiredDirection;
            set => _desiredDirection = value.normalized;
        }

        public bool CanAttack
        {
            get => _canAttack && NumberOfAttacking < MaxCanAttack;
            set => _canAttack = value;
        }

        public static LayerMask Layer { get; private set;  }
        
        public static int NumberOfAttacking { get; set; }
        public static int MaxCanAttack { get; set; } = 2;

        #endregion

        #region Events

        public event Action Enabled;

        #endregion

        #region Function Events

        protected override void Awake()
        {
            base.Awake();
            _roomEnemies = transform.parent.GetComponent<RoomEnemies>();
            _rigidbody = GetComponent<Rigidbody2D>();
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
                if(hittable == null) return;       
                
                hittable.OnHit(_damage);
            }
        }

        #endregion

        #region Public Methods

        public override void OnDie()
        {
            gameObject.SetActive(false);
            _roomEnemies.EnemyKilled();
        }

        #endregion

        #region Private Methods

        protected virtual void Move()
        {
            if(_rigidbody.bodyType != RigidbodyType2D.Static)
                _rigidbody.velocity = _desiredDirection * _speed;
        }

        #endregion
    }
}