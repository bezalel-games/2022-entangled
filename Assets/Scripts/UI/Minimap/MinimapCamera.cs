using System;
using Managers;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
  #region Serialized Fields
  
  #endregion
  #region Non-Serialized Fields

  private Transform _playerTransform;
  
  #endregion
  #region Properties
  
  #endregion
  #region Function Events

  private void Start()
  {
    _playerTransform = GameManager.PlayerTransform;
  }

  private void LateUpdate()
  {
    if (CameraManager.ZoomedIn)
    {
      if(transform.position != Vector3.zero) transform.position = Vector3.zero;
    }
    else
    {
      var pos = transform.position;
      var playerPos = _playerTransform.position;
      pos.x = playerPos.x;
      pos.y = playerPos.y;
      transform.position = pos;
    }
  }

  #endregion
  #region Public Methods
  
  #endregion
  #region Private Methods
  
  #endregion
}

