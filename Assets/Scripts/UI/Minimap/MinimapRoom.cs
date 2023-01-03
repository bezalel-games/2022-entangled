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
  
  #endregion
  #region Function Events
  
  #endregion
  #region Public Methods

  private void Awake()
  {
    Room room = GetComponentInParent<Room>();
    RoomNode node = room.Node;
    MinimapManager.AddRoom(node.Index);
    foreach (Direction dir in DirectionExt.GetDirections())
    {
      _connections[(byte) dir].SetActive(node[dir]!=null);
    }
  }

  private void Start()
  {
    transform.parent = MinimapManager.MinimapParent;
  }

  #endregion
  #region Private Methods
  
  #endregion
}

