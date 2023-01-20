using System;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
  #region Serialized Fields
  
  #endregion
  #region Non-Serialized Fields

  private static Dictionary<EffectType, string> AnimationNames = new()
  {
    {EffectType.ENEMY_HIT, "Base Layer.Enemy Hit"},
    {EffectType.WALL_HIT, "Base Layer.Wall Hit"},
    {EffectType.EXPLOSION, "Base Layer.Explosion Buff"},
    {EffectType.SHIELD_BREAK, "Base Layer.Shield Break"},
    {EffectType.STUN, "Base Layer.Stun"},
    {EffectType.TELEPORT, "Base Layer.Teleportation"}
  };

  private Animator _animator;

  #endregion
  #region Properties

  public EffectType Type
  {
    set => _animator.Play(AnimationNames[value]);
  }
  
  #endregion
  #region Function Events

  private void Awake()
  {
    _animator = GetComponent<Animator>();
  }

  #endregion
  #region Public Methods

  public void KillMe()
  {
    Destroy(gameObject);
  }
  
  #endregion
  #region Private Methods
  
  #endregion

  #region enums

  public enum EffectType
  {
    WALL_HIT, EXPLOSION, STUN, SHIELD_BREAK, ENEMY_HIT, TELEPORT, NONE
  }  

  #endregion
}

