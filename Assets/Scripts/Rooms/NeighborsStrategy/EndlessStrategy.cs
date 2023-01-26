using UnityEngine;
using Utils;

namespace Rooms.NeighborsStrategy
{
    public class EndlessStrategy : INeighborsStrategy
    {
        private const float INTENSITY_NORMALIZATION_FACTOR = 2 / Mathf.PI;

        #region INeighborsStrategy Implementation

        public float RoomIntensity(Vector2Int index) =>
            Mathf.Atan(Mathf.Sqrt(index.L1Norm())) * INTENSITY_NORMALIZATION_FACTOR;

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