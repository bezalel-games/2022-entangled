using System;
using UnityEngine;
using UnityEngine.UI;

namespace HP_System
{
    public abstract class LivingBehaviour : MonoBehaviourExt, IHittable
    {
        #region Serialized Fields

        [Header("Living Behaviour")] 
        [SerializeField] private float _maxHp;
        [SerializeField] private float _maxMp;

        protected readonly float PushbackTime = 0.3f;

        #endregion

        #region Non-Serialized Fields

        private float _hp;
        private float _mp;
        private float _hitTime;
        private float _pushbackFactor = 0.4f;
        
        protected Rigidbody2D Rigidbody;
        
        protected Vector2 PushbackVector;
        private Vector2 _pushbackDirection;
        
        protected Animator Animator;
        protected SpriteRenderer Renderer;

        #endregion

        #region Properties

        protected virtual bool Invulnerable { get; set; }

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

        protected float Hp
        {
            get => _hp;
            set
            {
                _hp = Mathf.Min(Mathf.Max(value, 0), _maxHp);
                
                OnHpChange?.Invoke(_hp, MaxHp);

                if (IsDead)
                {
                    OnDie();
                }
            }
        }

        protected float MaxMp
        {
            get => _maxMp;
            set
            {
                _maxMp = Mathf.Max(value, 0);
                Mp = Mathf.Min(Mp, _maxMp);
            }
        }

        protected float Mp
        {
            get => _mp;
            set
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
            print($"rigidBody: {Rigidbody}");
        }

        protected override void Update()
        {
            base.Update();

            if (Time.time > _hitTime + PushbackTime && PushbackVector != Vector2.zero)
            {
                PushbackVector = Vector2.zero;
            }
            if (Time.time >= _hitTime && Time.time <= _hitTime + PushbackTime)
            {
                var t = (Time.time - _hitTime) / PushbackTime;
                PushbackVector = Vector2.Lerp(_pushbackDirection, Vector2.zero, t);
                if (PushbackVector.magnitude <= 0.1f)
                {
                    PushbackVector = Vector2.zero;
                }
            }
        }

        #endregion

        #region Public Methods
        
        protected virtual void OnEnable()
        {
            Hp = MaxHp;
            Mp = MaxMp;
        }

        #endregion

        #region Private Methods

        #endregion

        #region IHittable

        public virtual void OnHit(Transform attacker, float damage)
        {
            if (Invulnerable || IsDead) return;
            Hp -= damage;

            _pushbackDirection = _pushbackFactor * (transform.position - attacker.position.normalized);
            _hitTime = Time.time;
        }

        public void OnHeal(float health)
        {
            Hp += health;
        }

        public abstract void OnDie();

        #endregion
    }
}