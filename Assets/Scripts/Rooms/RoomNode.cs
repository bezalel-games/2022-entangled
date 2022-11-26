using System;
using Rooms.CardinalDirections;
using UnityEngine;

namespace Rooms
{
    [Serializable]
    public class RoomNode
    {
        #region Serialized Fields

        [SerializeField] private Vector2Int index;
        [SerializeField] private Room room;

        #endregion

        #region Non-Serialized Fields

        [NonSerialized] private RoomNode[] _nodes;

        #endregion

        #region Properties

        public Room Room
        {
            get => room;
            set => room = value;
        }

        public Vector2Int Index => index;

        #endregion

        #region Indexers

        public RoomNode this[Direction dir]
        {
            get => _nodes[(byte)dir];
            set => _nodes[(byte)dir] = value;
        }

        #endregion

        #region Public Methods

        public RoomNode(Room room, Vector2Int index)
        {
            Room = room;
            this.index = index;
            _nodes = new RoomNode[4];
        }

        #endregion
    }
}