using UnityEngine;
using Utils;

namespace Rooms.NeighborsStrategy
{
    public class EndlessStrategy : INeighborsStrategy
    {
        #region INeighborsStrategy Implementation

        public int RoomRank(int minRoomRank, Vector2Int index, AnimationCurve distanceToRankFunction)
        {
            return minRoomRank + (int)Mathf.Log(1 + index.L1Norm());
        }

        public RoomType RoomType(Vector2Int index)
        {
            return Rooms.RoomType.MONSTERS;
        }

        #endregion
    }
}