using System;
using Rooms;
using Rooms.CardinalDirections;
using UnityEngine;

public class MinimapRoom : MonoBehaviour
{
  #region Serialized Fields

  [SerializeField] private GameObject[] _connections = new GameObject[4];

  #endregion
  #region Non-Serialized Fields
  
  #endregion
  #region Properties
  
  [field: SerializeField] public SpriteRenderer Renderer { get; private set; }
  public Vector2Int Index { get; private set; }
  
  #endregion
  #region Function Events
  
  #endregion
  #region Public Methods

  public void Init(Vector2Int index)
  {
    Index = index;
    Renderer.sprite = MinimapManager.Sprites[RoomManager.GetRoomType(Index)].before;
    SetConnections();
  }

  public void SetCleared()
  {
    Renderer.sprite = MinimapManager.Sprites[RoomManager.GetRoomType(Index)].after;
  }

  private void SetConnections()
  {
    foreach (Direction dir in DirectionExt.GetDirections())
    {
      var newIndex = Index + dir.ToVector();
      bool shouldShow = RoomManager.GetRoomType(newIndex) != RoomType.NONE && !MinimapManager.HasRoom(newIndex);
      _connections[(byte) dir].SetActive(shouldShow);
    }
  }

  #endregion
  #region Private Methods
  
  #endregion
}

