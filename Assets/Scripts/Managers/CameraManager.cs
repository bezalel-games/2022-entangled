using System;
using System.Collections;
using Rooms;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CameraManager : MonoBehaviour
{
  #region Serialized Fields

  [SerializeField] private Camera _mainCamera;
  [SerializeField] private Camera _minimapCamera;
  
  [Header("Minimap")]
  [SerializeField] private float _zoomOutSize;
  [SerializeField] private float _zoomInSize;

  [SerializeField] private RawImage _outMap;
  [SerializeField] private RawImage _inMap;

  [Header("Shake")] 
  [SerializeField] private float _playerHitDuration;
  [SerializeField] private float _playerHitMagnitude;
  
  [SerializeField] private float _enemyHitDuration;
  [SerializeField] private float _enemyHitMagnitude;
  
  
  #endregion
  #region Non-Serialized Fields

  private static CameraManager _instance;
  private Vector3 _mainCameraInitPos;
  private Vector3 _minimapCameraInitPos;
  private Coroutine _shakeRoutine;

  #endregion

  #region Properties
  
  public static bool ZoomedIn { get; private set; }

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
    _minimapCamera.orthographicSize = _zoomOutSize;

    _mainCameraInitPos = _mainCamera.transform.position;
    _minimapCameraInitPos = _minimapCamera.transform.position;
  }

  #endregion

  #region Public Methods

  public static void ToggleMinimap(bool zoomIn)
  {
    _instance.ToggleMinimapInner(zoomIn);
    ZoomedIn = zoomIn;
  }

  public static void PlayerHitShake()
  {
    StopShake();
    _instance._shakeRoutine = _instance.StartCoroutine(
      _instance.ShakeCamera(_instance._playerHitDuration, _instance._playerHitMagnitude));
  }
  
  public static void EnemyHitShake()
  {
    StopShake();
    _instance._shakeRoutine = _instance.StartCoroutine(
      _instance.ShakeCamera(_instance._enemyHitDuration, _instance._enemyHitMagnitude));
  }

  private static void StopShake()
  {
    var c = _instance._shakeRoutine;
    if (c != null)
    {
      _instance.StopCoroutine(c);
    }
  }

  #endregion

  #region Private Methods
  
  private void ToggleMinimapInner(bool zoomIn)
  {
    _minimapCamera.orthographicSize = zoomIn ? _zoomInSize : _zoomOutSize;
    if (zoomIn)
    {
      _inMap.gameObject.SetActive(true);
      _outMap.gameObject.SetActive(false);
    }
    else
    {
      _inMap.gameObject.SetActive(false);
      _outMap.gameObject.SetActive(true);
    }
  }

  private IEnumerator ShakeCamera(float duration, float magnitude)
  {
    var t = 0f;
    var start = magnitude;
    var end = 0;
    yield return null;
    
    while (t <= duration)
    {
      var gain = (1 - (t / duration)) * start + (t / duration) * end;
      RoomManager.CameraPerlin.m_AmplitudeGain = gain;
      t += Time.deltaTime;
      yield return null;
    }

    RoomManager.CameraPerlin.m_AmplitudeGain = 0;
    yield return null;
  }

  #endregion
}

