using UnityEngine;

namespace Rooms.NeighborsStrategy
{
    public interface INeighborsStrategy
    {
        public bool RoomExists(Vector2Int index);
        public bool IsBossRoom(Vector2Int index);
        public int RoomRank(int minRoomRank, Vector2Int index, AnimationCurve distanceToRankFunction);
    }
}