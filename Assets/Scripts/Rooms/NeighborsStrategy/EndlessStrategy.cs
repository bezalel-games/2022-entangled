using UnityEngine;
using Utils;

namespace Rooms.NeighborsStrategy
{
    public class EndlessStrategy : INeighborsStrategy
    {
        private const float INTENSITY_NORMALIZATION_FACTOR = 2 / Mathf.PI;
        private readonly int _distToSpecial = 7;

        public EndlessStrategy(int distToSpecial)
        {
            _distToSpecial = distToSpecial;
        }

        #region INeighborsStrategy Implementation

        public float RoomIntensity(Vector2Int index) =>
            Mathf.Atan(Mathf.Sqrt(index.L1Norm())) * INTENSITY_NORMALIZATION_FACTOR;

        public int RoomRank(int minRoomRank, int maxRoomRank, Vector2Int index, AnimationCurve distanceToRankFunction)
        {
            return minRoomRank + (int)Mathf.Log(1 + index.L1Norm());
        }

        public RoomType RoomType(Vector2Int index)
        {
            int x = index.x;
            int y = index.y;

            if (x%_distToSpecial == 0 && y%_distToSpecial == 0)
            {
                if (x/_distToSpecial == y/_distToSpecial || x == 0 || y == 0)
                {
                    return (x / _distToSpecial + y / _distToSpecial) % 2 == 0 ? Rooms.RoomType.TREASURE : Rooms.RoomType.FOUNTAIN;
                }
            }

            return Rooms.RoomType.MONSTERS;
        }

        public float GhostChance(int minRoomRank, int maxRoomRank, Vector2Int index, AnimationCurve distanceToRankFunction, float chanceFactor) => 0.5f;

        #endregion
    }
}