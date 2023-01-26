using System;
using Enemies;
using UnityEngine;
using Utils;

namespace HP_System
{
    public abstract class LivingBehaviour : MonoBehaviourExt, IBarrierable
    {
        #region Serialized Fields

        [Header("Living Behaviour")]
        [SerializeField] private float _maxHp;

        [SerializeField] private float _maxMp;
        [SerializeField] private float _pushbackFactor = 0.4f;
        [SerializeField] protected float _pushbackTime = 0.3f;
        [SerializeField] private GameObject _confusion;
        [field: SerializeField] protected ParticleSystem HitParticles { get; private set; }

        #endregion

        #region Non-Serialized Fields

        private float _hp;
        private float _mp;
        private float _hitTime;

        protected Rigidbody2D Rigidbody;

        protected Vector2 PushbackVector;
        private Vector2 _pushbackDirection;

        protected Animator Animator;
        protected SpriteRenderer Renderer;

        #endregion

        #region Properties

        [field: SerializeField] public SpiritualBarrier Barrier { get; private set; }
        public bool HasBarrier => Barrier != null && Barrier.Active;

        protected virtual bool Invulnerable { get; set; }

        protected virtual bool Frozen { get; set; }

        public event Action<float, float> OnHpChange;
        public event Action<float, float> OnMpChange;

        public bool IsDead => Hp <= 0;

        public float MaxHp
        {
            get => _maxHp;
            set
            {
                _maxHp = Mathf.Max(value, 0);
                Hp = Mathf.Min(Hp, _maxHp);
            }
        }

        public virtual float Hp
        {
            get => _hp;
            protected set
            {
                _hp = Mathf.Min(Mathf.Max(value, 0), _maxHp);

                OnHpChange?.Invoke(_hp, MaxHp);

                if (IsDead)
                {
                    OnDie();
                }
            }
        }

        public float MaxMp
        {
            get => _maxMp;
            set
            {
                _maxMp = Mathf.Max(value, 0);
                Mp = Mathf.Min(Mp, _maxMp);
            }
        }

        public float Mp
        {
            get => _mp;
            protected set
            {
                _mp = Mathf.Min(Mathf.Max(value, 0), _maxMp);

                OnMpChange?.Invoke(_mp, MaxMp);
            }
        }

        #endregion

        #region Animator Strings

        protected static readonly int Dash = Animator.StringToHash("dash");
        protected static readonly int Walking = Animator.StringToHash("walking");
        protected static readonly int xDirection = Animator.StringToHash("xDirection");
        protected static readonly int yDirection = Animator.StringToHash("yDirection");

        #endregion

        #region Function Events

        protected virtual void Awake()
        {
            Hp = MaxHp;
            Mp = MaxMp;

            Animator = GetComponentInChildren<Animator>();
            Renderer = GetComponentInChildren<SpriteRenderer>();
            Rigidbody = GetComponent<Rigidbody2D>();
        }

        protected override void Update()
        {
            base.Update();

            if (Time.time > _hitTime + _pushbackTime && PushbackVector != Vector2.zero)
            {
                PushbackVector = Vector2.zero;
            }

            if (Time.time >= _hitTime && Time.time <= _hitTime + _pushbackTime)
            {
                var t = (Time.time - _hitTime) / _pushbackTime;
                PushbackVector = Vector2.Lerp(_pushbackDirection, Vector2.zero, t);
                if (PushbackVector.magnitude <= 0.1f)
                {
                    PushbackVector = Vector2.zero;
                }
            }
        }

        protected virtual void OnEnable()
        {
            Hp = MaxHp;
            Mp = MaxMp;
        }

        #endregion

        #region Public Methods

        public virtual void Stop()
        {
            Rigidbody.velocity = Vector2.zero;
        }

        public virtual void Stun(float duration)
        {
            Stop();
            _confusion.gameObject.SetActive(true);
            Frozen = true;
            DelayInvoke((() =>
            {
                _confusion.gameObject.SetActive(false);
                Frozen = false;
            }), duration);
        }

        #endregion

        #region Private Methods

        private void EmitParticles(Transform attacker)
        {
            if (HitParticles == null)
                return;

            Vector2 dir = (attacker.position - transform.position).normalized;
            var angles = dir.Angles(); //add 90 since emission is aimed down originally
            HitParticles.transform.eulerAngles = Vector3.forward * (angles + 90);
            HitParticles.Play();
        }

        #endregion

        #region IHittable

        public virtual void OnHit(Transform attacker, float damage, bool pushBack = true)
        {
            if (Invulnerable || IsDead || HasBarrier) return;
            Hp -= damage;

            if (!attacker)
                return;

            HitShake();
            if (pushBack)
                _pushbackDirection = _pushbackFactor * (transform.position - attacker.position).normalized;
            _hitTime = Time.time;
            EmitParticles(attacker);
        }

        protected abstract void HitShake();

        public void OnHeal(float health)
        {
            Hp += health;
        }

        public abstract void OnDie();

        #endregion
    }
}