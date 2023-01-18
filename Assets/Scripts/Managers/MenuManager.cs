using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
  #region Serialized Fields
  
  #endregion
  #region Non-Serialized Fields

  private static MenuManager _instance;

  #endregion

  #region Properties

  #endregion

  #region Function Events

  private void Awake()
  {
    if (_instance != null)
    {
      Destroy(gameObject);
      return;
    }

    _instance = this;
  }

  #endregion

  #region Public Methods

  public static void StartRun()
  {
    SceneManager.LoadScene("Run Scene");
  }
  
  public static void StartEndless()
  {
    SceneManager.LoadScene("Endless Scene");
  }

  #endregion

  #region Private Methods

  #endregion
}

