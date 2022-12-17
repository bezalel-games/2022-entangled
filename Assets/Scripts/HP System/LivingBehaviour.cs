using UnityEngine;
using UnityEngine.UI;

namespace HP_System
{
    public abstract class LivingBehaviour : MonoBehaviourExt, IHittable
    {
        #region Serialized Fields

        [Header("Living Behaviour")] [SerializeField]
        private float _maxHp;

        [SerializeField] private Image _healthBar;

        [SerializeField] private float _maxMp;
        [SerializeField] private Image _mpBar;

        #endregion

        #region Non-Serialized Fields

        private float _hpBarWidth;
        private RectTransform _hpBarTransform;
        private float _hp;

        private float _mpBarWidth;
        private RectTransform _mpBarTransform;
        private float _mp;

        #endregion

        #region Properties

        protected bool Invulnerable { get; set; }
        
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

                UpdateHealthUI();

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

                UpdateMPUI();
            }
        }

        #endregion

        #region Function Events

        protected virtual void Awake()
        {
            if (_healthBar != null)
            {
                _hpBarTransform = _healthBar.rectTransform;
                _hpBarWidth = _hpBarTransform.rect.size.x;
            }

            if (_mpBar != null)
            {
                _mpBarTransform = _mpBar.rectTransform;
                _mpBarWidth = _mpBarTransform.rect.size.x;
            }

            Hp = MaxHp;
            Mp = MaxMp;
            print($"start MP: {Mp}");
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

        protected virtual void UpdateHealthUI()
        {
            if (_healthBar == null) return;

            SetRectRation(_hpBarTransform, _hpBarWidth, Hp, MaxHp);
            // var ratio = 1 - (Hp / MaxHp);
            // var newWidth = -1 * ratio * _hpBarWidth;
            // _hpBarTransform.sizeDelta = new Vector2(newWidth, _hpBarTransform.sizeDelta.y);
        }

        protected virtual void UpdateMPUI()
        {
            if (_mpBar == null) return;

            SetRectRation(_mpBarTransform, _mpBarWidth, Mp, MaxMp);
        }

        private void SetRectRation(RectTransform rectTransform, float maxWidth, float currValue, float maxValue)
        {
            var ratio = 1 - (currValue / maxValue);
            var newWidth = -1 * ratio * maxWidth;
            rectTransform.sizeDelta = new Vector2(newWidth, rectTransform.sizeDelta.y);
        }

        #endregion
    }
}