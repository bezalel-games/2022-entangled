using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class LivingBehaviour : MonoBehaviourExt, Hittable
{
  #region Serialized Fields

  [Header("Living Behaviour")]
  [SerializeField] private float _maxHp;
  [SerializeField] private Image _healthBar;
  
  #endregion
  #region Non-Serialized Fields

  private float _barWidth;
  private RectTransform _barTransform;
  
  private float _hp;

  #endregion
  
  #region Properties
  
  protected bool Invulnerable { get; set; }

  protected float MaxHP
  {
    get => _maxHp;
    set
    {
      _maxHp = Mathf.Max(value, 0);
      HP = Mathf.Min(HP, _maxHp);
    }
  }

  protected float HP
  {
    get => _hp;
    set
    {
      print($"{_maxHp}, {_hp} {value}");
      _hp = Mathf.Min(Mathf.Max(value, 0), _maxHp);
      
      UpdateHealthUI();
      
      if (_hp <= 0)
      {
        OnDie();
      }
    }
  }
  
  #endregion
  #region Function Events

  protected virtual void Awake()
  {
    if (_healthBar != null)
    {
      _barTransform = _healthBar.rectTransform;
      _barWidth = _barTransform.rect.size.x;
    }

    HP = MaxHP;
  }

  #endregion
  #region Public Methods
  
  #endregion
  #region Private Methods
  
  #endregion
  
  #region Hitable
  
  public void OnHit(float damage)
  {
    if (Invulnerable) return;
    HP -= damage; 
  }

  public void OnHeal(float health) { HP += health; }
  public abstract void OnDie();

  protected virtual void UpdateHealthUI()
  {
    if(_healthBar == null) return;
    
    var ratio = 1 - (HP / MaxHP);
    var newWidth = -1 * ratio * _barWidth;
    _barTransform.sizeDelta = new Vector2(newWidth, _barTransform.sizeDelta.y);
  }

  #endregion
}

