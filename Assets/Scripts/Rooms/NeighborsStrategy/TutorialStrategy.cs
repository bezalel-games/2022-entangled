using UnityEngine;

namespace Rooms.NeighborsStrategy
{
    public class TutorialStrategy : INeighborsStrategy
    {
        private int _length;
        
        public TutorialStrategy(int length)
        {
            _length = length;
        }

        public float RoomIntensity(Vector2Int index)
        {
            return 0;
        }
        
        public RoomType RoomType(Vector2Int index)
        {
            return (index.x == 0 && index.y >= 0 && index.y <= _length) 
                ? Rooms.RoomType.TUTORIAL
                : Rooms.RoomType.NONE;
        }

        public int RoomRank(int minRoomRank, int maxRoomRank, Vector2Int index, AnimationCurve distanceToRankFunction)
        {
            return 0;
        }
    }
}