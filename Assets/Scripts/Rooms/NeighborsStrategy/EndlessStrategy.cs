using UnityEngine;

namespace Rooms.NeighborsStrategy
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