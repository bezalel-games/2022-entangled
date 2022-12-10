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
  [field: SerializeField] public float AttackTime { get; private set; }
  [field: SerializeField] public float AttackDistance { get; private set; }
  [field: SerializeField] public float FollowDistance { get; private set; }

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
    print($"override. attacking={_attacking}");
    if (!_attacking)
    {
      base.Move();
    }
    else
    {
      print("attack vel");
      _rigidbody.velocity = DesiredDirection * _attackSpeed;
    }
  }

  #endregion
}

