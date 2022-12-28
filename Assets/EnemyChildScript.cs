using System;
using Enemies;
using UnityEngine;

public class EnemyChildScript : MonoBehaviour
{
  #region Serialized Fields
  
  #endregion
  #region Non-Serialized Fields

  private Enemy _parent;

  #endregion

  #region Properties

  #endregion

  #region Function Events

  private void Awake()
  {
    _parent = GetComponentInParent<Enemy>();
  }

  #endregion

  #region Public Methods

  public void Die()
  {
    _parent.AfterDeathAnimation();
  }

  #endregion

  #region Private Methods

  #endregion
}

