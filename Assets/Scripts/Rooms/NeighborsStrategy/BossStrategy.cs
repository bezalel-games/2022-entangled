using UnityEngine;

namespace Rooms.NeighborsStrategy
{
    public class BossStrategy : INeighborsStrategy
    {
        private static readonly Vector2Int BossRoom = new Vector2Int(0, 1);
        public bool RoomExists(Vector2Int index)
        {
            return index == BossRoom;
        }

        public bool IsBossRoom(Vector2Int index)
        {
            return index == BossRoom;
        }

        public int RoomRank(int minRoomRank, Vector2Int index, AnimationCurve distanceToRankFunction)
        {
            return 0;
        }
    }
}