using Cards.Buffs.Components;
using HP_System;
using Player;
using UnityEngine;

namespace Enemies.Boss
{
    public class Bomb : MonoBehaviour, IHittable
    {
        private SpiritualBarrier _barrier;
        private Rigidbody2D _rigidBody;

        #region Serialized Fields

        [SerializeField] private float _timeToExplosion;
        [SerializeField] private float _scaleSpeed;
        [Range(0, 2)] [SerializeField] private float _maxAdditionalScale = 0.3f;
        [SerializeField] private Explosion _explosionPrefab;

        #endregion

        #region Non-Serialized Fields

        private float _explosionTime;
        [SerializeField] private float _pushbackFactor = 10;
        private Vector3 _baseScale;
        private bool _active = false;
        
        #endregion

        #region Properties

        public bool Active
        {
            get => _active;
            set
            {
                if (!_active && value)
                    _explosionTime = Time.time + _timeToExplosion;
                _active = value;
            }
        }
        private float TimeLeftToExplosion => _explosionTime - Time.time;

        #endregion

        #region Function Events

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _barrier = GetComponentInChildren<SpiritualBarrier>();
            _barrier.Active = true;
            _baseScale = transform.localScale;
        }

        private void Update()
        {
            if (!Active) return;
            if (TimeLeftToExplosion <= 0)
            {
                Instantiate(_explosionPrefab);
                Destroy(gameObject);
            }
            var sinTSquared = Mathf.Sin(Mathf.Pow(_scaleSpeed * (1 - TimeLeftToExplosion / _timeToExplosion), 2));
            var adjustedSinTSquared = (sinTSquared + 1) * _maxAdditionalScale / 2;
            transform.localScale = _baseScale + Vector3.one * adjustedSinTSquared;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Precision")) return;
            if (other.GetComponent<Line>() == null || other is not PolygonCollider2D) return;
            _barrier.Active = false;
        }

        #endregion

        #region IHittable Implementation

        public virtual void OnHit(Transform attacker, float damage, bool pushBack = true)
        {
            if (_barrier.Active) return;
            if (!attacker)
                return;
            if (pushBack)
                _rigidBody.AddForce(_pushbackFactor * (transform.position - attacker.position).normalized);
            // EmitParticles(attacker);
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion
    }
}