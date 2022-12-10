using System;
using UnityEngine;

namespace Enemies
{
    public class Enemy : MonoBehaviourExt
    {
        #region Serialized Fields

        [Header("Enemy")] 
        [SerializeField] private float _speed;

        #endregion

        #region Non-Serialized Fields

        private RoomEnemies _roomEnemies;
        protected Rigidbody2D _rigidbody;
        private Vector2 _desiredDirection;

        #endregion

        #region Properties

        [field: SerializeField] public int Rank { get; set; } = 1;

        public Vector2 DesiredDirection
        {
            get => _desiredDirection;
            set => _desiredDirection = value.normalized;
        }

        public static LayerMask Layer { get; private set;  }

        #endregion

        #region Function Events

        protected virtual void Awake()
        {
            _roomEnemies = transform.parent.GetComponent<RoomEnemies>();
            _rigidbody = GetComponent<Rigidbody2D>();
            Layer = LayerMask.GetMask("Enemies");
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            _roomEnemies.EnemyKilled();
            gameObject.SetActive(false);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            Move();
        }

        #endregion

        #region Public Methods

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