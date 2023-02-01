using System.Collections.Generic;
using HP_System;
using UnityEngine;

namespace Cards.Buffs.Components
{
    public class Explosion : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private AnimationCurve _explosionExpansionCurve;
        [SerializeField] private float _expansionTime = 1;
        [SerializeField] private bool _playerExplosion = true;

        #endregion

        #region Non-Serialized Fields

        private SpriteRenderer _spriteRenderer;
        private Rigidbody2D _rigidbody;
        private HashSet<IHittable> _hitObjects;
        private float _startTime;
        private bool _expanding;
        [SerializeField] private bool _canHitPlayer;

        #endregion

        #region Properties

        [field: SerializeField] public float Radius { get; set; } = 1f;
        [field: SerializeField] public float Damage { get; set; } = 1;

        #endregion

        #region Function Events

        private void OnEnable()
        {
            transform.SetParent(null);
            _hitObjects = new HashSet<IHittable>();
            transform.localScale = Vector3.one;
            _startTime = Time.time;
        }

        private void Update()
        {
            var t = (Time.time - _startTime) / _expansionTime;
            if (t < 1)
            {
                transform.localScale = Vector3.one * (Radius * _explosionExpansionCurve.Evaluate(t));
                return;
            }

            transform.localScale = Vector3.one * Radius;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_canHitPlayer && other.CompareTag("Player")) return;

            var hittableObj = other.GetComponent<IHittable>();
            if (hittableObj == null || _hitObjects.Contains(hittableObj)) return;
            hittableObj.OnHit(transform, Damage, explosion: _playerExplosion);
            _hitObjects.Add(hittableObj);
        }

        #endregion

        #region Public Methods

        public void DestroyObject()
        {
            Destroy(gameObject);
        }

        #endregion
    }
}