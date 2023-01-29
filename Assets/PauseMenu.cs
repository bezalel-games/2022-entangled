using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
  #region Serialized Fields

  [SerializeField] private Button _firstSelected;
  
  #endregion
  #region Non-Serialized Fields
  
  #endregion
  #region Properties
  
  #endregion
  #region Function Events

  private void OnEnable()
  {
    _firstSelected.Select();
  }

  #endregion
  #region Public Methods

  public void ResumeGame()
  {
    GameManager.Pause(false);
  }

  public void ExitGame()
  {
    LoadManager.LoadMenu();
  }
  
  #endregion
  #region Private Methods
  
  #endregion
}

