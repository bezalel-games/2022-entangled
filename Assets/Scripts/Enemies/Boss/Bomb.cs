using Audio;
using Cards.Buffs.Components;
using HP_System;
using Player;
using UnityEngine;

namespace Enemies.Boss
{
    public class Bomb : MonoBehaviour, IBarrierable
    {

        #region Serialized Fields

        [SerializeField] private float _timeToExplosion;
        [SerializeField] private float _scaleSpeed;
        [Range(0, 2)] [SerializeField] private float _maxAdditionalScale = 0.3f;
        [SerializeField] private float _pushbackFactor = 10;
        [SerializeField] private Explosion _explosionPrefab;
        [Tooltip("Damage given to player on touch")][SerializeField] private float _touchDamage = 5;

        #endregion

        #region Non-Serialized Fields

        private float _explosionTime;
        private Vector3 _baseScale;
        private bool _active = false;
        
        private SpiritualBarrier _barrier;
        private Rigidbody2D _rigidBody;
        private Collider2D _collider;
        private SpriteRenderer _spriteRenderer;

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

        public bool HasBarrier => _barrier.Active;

        #endregion

        #region Function Events

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _barrier = GetComponentInChildren<SpiritualBarrier>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _barrier.Active = true;
            _baseScale = transform.localScale;
        }

        private void Update()
        {
            if (!Active) return;
            if (TimeLeftToExplosion <= 0)
            {
                AudioManager.PlayOneShot(SoundType.ENEMY, (int)EnemySounds.BOSS_EXPLOSION);
                Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            var sinTSquared = Mathf.Sin(Mathf.Pow(_scaleSpeed * (1 - TimeLeftToExplosion / _timeToExplosion), 2));
            var adjustedSinTSquared = (sinTSquared + 1) * _maxAdditionalScale / 2;
            transform.localScale = _baseScale + Vector3.one * adjustedSinTSquared;
            _spriteRenderer.color = Color.HSVToRGB(0,adjustedSinTSquared,1, true);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Precision")) return;
            if (other.GetComponent<Line>() == null || other is not PolygonCollider2D) return;
            _barrier.Active = false;
            AudioManager.PlayOneShot(SoundType.SFX, (int) SfxSounds.SHIELD_BREAK);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _collider.isTrigger = false;
        }
        
        protected virtual void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                var hittable = other.gameObject.GetComponent<IHittable>();
                if (hittable == null) return;

                hittable.OnHit(transform, _touchDamage);
            }
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