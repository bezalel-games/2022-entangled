using System;
using UnityEngine;

public class BackToMenu : MonoBehaviour
{
  #region Serialized Fields

  [SerializeField] private float _time;

  #endregion

  #region Non-Serialized Fields

  private float _menuTime;

  #endregion

  #region Properties

  #endregion

  #region Function Events

  private void Awake()
  {
    _menuTime = Time.time + _time;
  }

  private void Update()
  {
    if (Time.time > _menuTime)
    {
      LoadManager.LoadMenu();
      Destroy(gameObject);
    }
  }

  #endregion

  #region Public Methods

  #endregion

  #region Private Methods

  #endregion
}

