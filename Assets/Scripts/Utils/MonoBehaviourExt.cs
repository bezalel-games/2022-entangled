using System;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class MonoBehaviourExt : MonoBehaviour
{
  #region Serialized Fields
  
  #endregion
  #region Non-Serialized Fields

  private Dictionary<Action, float> _actions = new();
  private Dictionary<Action, float> _fixedActions = new();

  #endregion

  #region Properties

  #endregion

  #region Function Events

  protected virtual void Update()
  {
    if(_actions.Count > 0)
      CountDown(Time.deltaTime, _actions);
  }

  protected virtual void FixedUpdate()
  {
    if(_fixedActions.Count > 0)
      CountDown(Time.fixedDeltaTime, _fixedActions);
  }

  #endregion

  #region Public Methods

  #endregion

  #region Private Methods

  public void DelayInvoke(Action action, float delayTime)
  {
    _actions[action] = delayTime;
  }
  
  public void FixedDelayInvoke(Action action, float delayTime)
  {
    _fixedActions[action] = delayTime;
  }

  private void CountDown(float deltaTime, Dictionary<Action, float> actions)
  {
    var keys = new List<Action>(actions.Keys);
    foreach (var action in keys)
    {
      if(!actions.ContainsKey(action)) continue;
      
      var time = actions[action];
      var newTime = time - deltaTime;
      
      if (newTime <= 0)
      {
        action.Invoke();
        actions.Remove(action);
      }
      else
      {
        actions[action] = newTime;
      }
    }
  }

  #endregion
}

