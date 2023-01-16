using System;
using System.Collections.Generic;
using Rooms;
using UnityEngine;
using UnityEngine.UI;

public class MinimapManager : MonoBehaviour
{
  #region Serialized Fields
  
  [SerializeField] private Camera _minimapCamera;

  [SerializeField] private MinimapRoom _minimapRoomPrefab;

  [SerializeField] private List<MinimapPair> _minimapSprites = new();

  #endregion
  #region Non-Serialized Fields
  
  private static MinimapManager _instance;
  private readonly Dictionary<Vector2Int, MinimapRoom> _rooms = new();
  private readonly Dictionary<RoomType, (Sprite before, Sprite after)> _sprites = new();

  #endregion
  #region Properties

  public static Dictionary<RoomType, (Sprite before, Sprite after)> Sprites => _instance._sprites;
  public static Transform MinimapParent => _instance.transform;
  public static MinimapRoom MinimapRoomPrefab => _instance._minimapRoomPrefab;

  private static bool ShouldCreate(Vector2Int index) => !HasRoom(index) && RoomManager.GetRoomType(index) != RoomType.NONE;
  
  public static bool HasRoom(Vector2Int index) => _instance._rooms.ContainsKey(index);
  
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

    foreach (MinimapPair pair in _minimapSprites) { _sprites[pair._roomType] = (pair._spriteBefore, pair._spriteAfter); }
  }

  #endregion
  #region Public Methods

  public static void AddRoom(Vector2Int index)
  {
    if(!ShouldCreate(index))
      return;
    _instance.AddRoom_Inner(index);
  }

  public static void SetCleared(Vector2Int index)
  {
    if(!HasRoom(index))
      return;
    _instance._rooms[index].SetCleared();
  }

  private void AddRoom_Inner(Vector2Int index)
  {
    var minimap = Instantiate(_minimapRoomPrefab, RoomManager.GetPosition(index), Quaternion.identity, transform);
    minimap.Init(index);
    _instance._rooms[index] = minimap;
  }

  #endregion
  #region Private Methods

  #endregion

  #region Classes

  [Serializable]
  public struct MinimapPair
  {
    public RoomType _roomType;
    public Sprite _spriteBefore;
    public Sprite _spriteAfter;
  }

  #endregion
}

