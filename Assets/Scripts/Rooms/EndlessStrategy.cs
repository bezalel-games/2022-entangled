using Rooms.CardinalDirections;
using UnityEngine;

namespace Rooms
{
    public class EndlessStrategy : INeighborsStrategy
    {
        public bool RoomExists(Vector2Int index)
        {
            return true;
        }

        public bool IsBossRoom(Vector2Int index)
        {
            return false;
        }
    }
}