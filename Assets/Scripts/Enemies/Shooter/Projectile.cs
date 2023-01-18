using System;
using HP_System;
using Managers;
using Player;
using UnityEngine;

namespace Enemies
{
    public class Projectile : MonoBehaviour
    {
        #region Serialized Fields

        #endregion

        #region Non-Serialized Fields

        private Rigidbody2D _rigidbody;
        private Vector2 _direction;
        private float _speed;
        private float _lifeTime = 10f;

        private Transform _player;
        private float _disappearTime;

        #endregion

        #region Properties
        
        public float RotationSpeed { get; set; }

        public float Lifetime
        {
            get => _lifeTime;
            set
            {
                _lifeTime = value;
                _disappearTime = Time.time + _lifeTime;
            }
        }

        public Vector2 Direction
        {
            get => _direction;
            set
            {
                _direction = value.normalized;
                var angle = Vector2.SignedAngle(Vector2.up, _direction);
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }

        public float Speed
        {
            get => _speed;
            set => _speed = Mathf.Max(value, 0);
        }

        public float Damage { get; set; }

        public Action OnDisappear { get; set; }
        public bool Homing { get; set; }

        #endregion

        #region Function Events

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            _player = GameManager.PlayerTransform;
        }

        private void FixedUpdate()
        {
            var t = Homing ? RotationSpeed : 0;
            
            var playerDir = (Vector2) (_player.position - transform.position);
            Direction = 
                Vector2.Lerp(Direction.normalized, playerDir.normalized, t * playerDir.magnitude * Time.deltaTime);
            _rigidbody.velocity = Direction * _speed;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                var hittable = other.gameObject.GetComponent<IHittable>();
                hittable.OnHit(transform, Damage);
            }

            OnDisappear?.Invoke();
        }

        private void OnBecameInvisible()
        {
            OnDisappear?.Invoke();
        }

        private void OnEnable()
        {
            _disappearTime = Time.time + Lifetime;
        }

        private void Update()
        {
            if (Time.time > _disappearTime)
            {
                OnDisappear?.Invoke();
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion
    }
}