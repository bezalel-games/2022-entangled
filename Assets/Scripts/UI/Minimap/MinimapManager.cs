using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapManager : MonoBehaviour
{
  #region Serialized Fields
  
  [SerializeField] private Camera _minimapCamera;
  [SerializeField] private float _zoomOutSize;
  [SerializeField] private float _zoomInSize;

  [SerializeField] private RawImage _outMap;
  [SerializeField] private RawImage _inMap;

  [SerializeField] private MinimapRoom _minimapRoomPrefab;

  #endregion
  #region Non-Serialized Fields
  
  private static MinimapManager _instance;
  private HashSet<Vector2Int> _rooms = new HashSet<Vector2Int>();

  #endregion
  #region Properties

  public static Transform MinimapParent => _instance.transform;
  public static MinimapRoom MinimapRoomPrefab => _instance._minimapRoomPrefab;
  
  public static bool ZoomedIn { get; private set; }
  public static bool HasRoom(Vector2Int index) => _instance._rooms.Contains(index);
  
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
  }

  #endregion
  #region Public Methods
  
  public static void ToggleMinimap(bool zoomIn)
  {
    _instance.ToggleMinimapInner(zoomIn);
    ZoomedIn = zoomIn;
  }

  public static void AddRoom(Vector2Int index)
  {
    _instance._rooms.Add(index);
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
  
  #endregion
}

