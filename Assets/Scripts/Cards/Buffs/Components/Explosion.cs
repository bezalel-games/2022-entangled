using HP_System;
using UnityEngine;

namespace Cards.Buffs.Components
{
    public class Explosion : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private AnimationCurve _explosionExpansionCurve;
        [SerializeField] private float _expansionTime = 1;
        [SerializeField] private float _dissolveTime = 0.5f;

        #endregion

        #region Non-Serialized Fields

        private SpriteRenderer _spriteRenderer;
        private Rigidbody2D _rigidbody;
        private bool _expanding = true;
        private float _startTime;

        #endregion

        #region Properties

        public float Radius { get; set; }
        public float Damage { get; set; } = 1;

        #endregion

        #region Function Events

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            _rigidbody.velocity = transform.parent.GetComponent<Rigidbody2D>().velocity;
            transform.SetParent(null);
            _expanding = true;
            _startTime = Time.time;
            transform.localScale = Vector3.one * 0.01f;
        }

        private void Update()
        {
            if (_expanding)
            {
                var t = (Time.time - _startTime) / _expansionTime;
                if (t < 1)
                {
                    transform.localScale = Vector3.one * (Radius * _explosionExpansionCurve.Evaluate(t));
                    return;
                }

                transform.localScale = Vector3.one * Radius;
                _expanding = false;
                return;
            }

            if (_spriteRenderer.color.a <= 0)
                Destroy(gameObject);
            _spriteRenderer.color -= Color.black * Time.deltaTime / _dissolveTime;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
                other.GetComponent<IHittable>()?.OnHit(transform, Damage);
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion
    }
}