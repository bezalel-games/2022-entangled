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
  
  private bool _following;
  
  #endregion
  #region Properties

  public bool PreparingAttack { get; set; } = false;

  #endregion
  #region Function Events

  #endregion
  #region Public Methods

  public void Attack(Action onEnd)
  {
    DelayInvoke((() => { Attacking = false; onEnd?.Invoke();}), AttackTime);
  }

  #endregion
  
  #region Private Methods

  protected override void Move()
  {
    if (!Attacking || IsDead)
    {
      base.Move();
    }
    else
    {
      if(!PreparingAttack)
        Rigidbody.velocity = DesiredDirection * _attackSpeed;
    }
  }

  #endregion
}

