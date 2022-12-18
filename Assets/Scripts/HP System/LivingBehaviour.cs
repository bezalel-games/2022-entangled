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

        #endregion

        #region Non-Serialized Fields

        private float _hp;
        private float _mp;

        #endregion

        #region Properties

        protected bool Invulnerable { get; set; }

        public event Action<float, float> OnHpChange;
        public event Action<float, float> OnMpChange;
        
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

                if (_hp <= 0)
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

        #region Function Events

        protected virtual void Awake()
        {
            Hp = MaxHp;
            Mp = MaxMp;
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

        public virtual void OnHit(float damage)
        {
            if (Invulnerable) return;
            Hp -= damage;
        }

        public void OnHeal(float health)
        {
            Hp += health;
        }

        public abstract void OnDie();

        #endregion
    }
}