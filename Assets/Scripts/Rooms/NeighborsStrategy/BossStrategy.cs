using UnityEngine;

namespace Rooms.NeighborsStrategy
{
    public class BossStrategy : INeighborsStrategy
    {
        private static readonly Vector2Int BossRoom = new Vector2Int(0, 1);

        #region INeighborsStrategy Implementation

        public RoomType RoomType(Vector2Int index)
        {
            return index == Vector2Int.zero ? Rooms.RoomType.START :
                index == BossRoom ? Rooms.RoomType.BOSS : Rooms.RoomType.NONE;
        }

        public int RoomRank(int minRoomRank, Vector2Int index, AnimationCurve distanceToRankFunction) => 0;

        #endregion
    }
}