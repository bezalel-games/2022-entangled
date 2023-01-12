using UnityEngine;
using Utils;

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

        public int RoomRank(int minRoomRank, Vector2Int index, AnimationCurve distanceToRankFunction)
        {
            return minRoomRank + (int)Mathf.Log(1 + index.L1Norm());
        }
    }
}