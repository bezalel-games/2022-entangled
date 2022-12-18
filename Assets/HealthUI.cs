using System;
using HP_System;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
  #region Serialized Fields

  [SerializeField] private Image _hpBar;
  [SerializeField] private Image _mpBar;
  [SerializeField] LivingBehaviour _living;


  #endregion

  #region Non-Serialized Fields

  private float _hpBarWidth;
  private RectTransform _hpBarTransform;
  private float _mpBarWidth;
  private RectTransform _mpBarTransform;

  #endregion

  #region Properties

  #endregion

  #region Function Events

  private void Awake()
  {
    if (_hpBar != null)
    {
      _hpBarTransform = _hpBar.rectTransform;
      _hpBarWidth = _hpBarTransform.rect.size.x;
    }

    if (_mpBar != null)
    {
      _mpBarTransform = _mpBar.rectTransform;
      _mpBarWidth = _mpBarTransform.rect.size.x;
    }
    
    if (_living != null)
    {
      _living.OnHpChange += UpdateHPUI;
      _living.OnMpChange += UpdateMPUI;
    }
  }

  private void OnDestroy()
  {
    if (_living != null)
    {
      _living.OnHpChange -= UpdateHPUI;
      _living.OnMpChange -= UpdateMPUI;
    }
  }

  #endregion

  #region Public Methods

  #endregion

  #region Private Methods
  
  protected virtual void UpdateHPUI(float hp, float maxHp)
  {
    if (_hpBar == null) return;

    SetRectRation(_hpBarTransform, _hpBarWidth, hp, maxHp);
  }

  protected virtual void UpdateMPUI(float mp, float maxMp)
  {
    if (_mpBar == null) return;

    SetRectRation(_mpBarTransform, _mpBarWidth, mp, maxMp);
  }

  private void SetRectRation(RectTransform rectTransform, float maxWidth, float currValue, float maxValue)
  {
    var ratio = 1 - (currValue / maxValue);
    var newWidth = -1 * ratio * maxWidth;
    rectTransform.sizeDelta = new Vector2(newWidth, rectTransform.sizeDelta.y);
  }

  #endregion
}

