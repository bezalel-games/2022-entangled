using System;
using Enemies;
using UnityEngine;

public class Goomba : Enemy
{
  #region Serialized Fields
  
  [Header("Goomba")]
  [SerializeField] private float _attackSpeed;

  #endregion
  #region Non-Serialized Fields

  private bool _attacking;
  private bool _following;
  
  #endregion
  #region Properties

  #endregion
  #region Function Events

  #endregion
  #region Public Methods

  public void Attack(Action onEnd)
  {
    _attacking = true;
    DelayInvoke((() => { _attacking = false; onEnd?.Invoke();}), AttackTime);
  }

  #endregion
  
  #region Private Methods

  protected override void Move()
  {
    if (!_attacking)
    {
      base.Move();
    }
    else
    {
      _rigidbody.velocity = DesiredDirection * _attackSpeed;
    }
  }

  #endregion
}

