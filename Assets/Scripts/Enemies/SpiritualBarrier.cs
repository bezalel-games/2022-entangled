using System;
using UnityEngine;

public class SpiritualBarrier : MonoBehaviour
{
  #region Serialized Fields

  [SerializeField] private Material _regularMaterial;
  [SerializeField] private Material _ghostMaterial;

  #endregion

  #region Non-Serialized Fields

  private SpriteRenderer _renderer;

  #endregion
  
  #region Function Events

  private void Awake()
  {
    _renderer = GetComponentInParent<SpriteRenderer>();
  }

  private void OnEnable()
  {
    if(_renderer == null) return;
    _renderer.material = _ghostMaterial;
  }

  private void OnDisable()
  {
    if(_renderer == null) return;
    _renderer.material = _regularMaterial;
  }

  #endregion
}

