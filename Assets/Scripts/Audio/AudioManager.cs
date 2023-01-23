using System;
using Audio;
using UnityEngine;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
  #region Serialized Fields

  [SerializeField] private AudioBank _bank;
  
  #endregion
  #region Non-Serialized Fields
  
  private static AudioManager _instance;
   
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

  public static void PlayOneShot(SoundType type, int val)
  {
    RuntimeManager.PlayOneShot(_instance._bank[type,val]);
  }
  
  #endregion
  #region Private Methods
  
  #endregion
}

