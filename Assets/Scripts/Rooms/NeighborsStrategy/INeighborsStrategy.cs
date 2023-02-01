using UnityEngine;

namespace Rooms.NeighborsStrategy
{
    public interface INeighborsStrategy
    {
        #region Abstract Methods

        public RoomType RoomType(Vector2Int index);
        
        // number in the range [0,1]. 0 is not intense at all, 1 is boss level intensity. 
        public float RoomIntensity(Vector2Int index);
        public int RoomRank(int minRoomRank, int maxRoomRank, Vector2Int index, AnimationCurve distanceToRankFunction);
        public float GhostChance(int minRoomRank, int maxRoomRank, Vector2Int index, AnimationCurve distanceToRankFunction, float chanceFactor);

        #endregion

        #region Default Methods

        public bool RoomExists(Vector2Int index) => RoomType(index) != Rooms.RoomType.NONE;
        public bool IsBossRoom(Vector2Int index) => RoomType(index) == Rooms.RoomType.BOSS;

        #endregion
    }
}