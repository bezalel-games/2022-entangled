using System.Collections.Generic;
using Rooms.CardinalDirections;
using UnityEngine;

namespace Rooms
{
    public interface INeighborsStrategy
    {
        public bool RoomExists(Vector2Int index);
        public bool IsBossRoom(Vector2Int index);
    }
}