using System;
using System.Collections;
using UnityEngine;
using static Rooms.CardinalDirections.Direction;

namespace Rooms.CardinalDirections
{
    public static class DirectionExt
    {
        private const int NumOfDirections = 4;

        public static IEnumerable GetDirections() => (Direction[])Enum.GetValues(typeof(Direction));

        public static Direction Inverse(this Direction dir)
        {
            return (Direction)(((byte)dir + 2) % NumOfDirections);
        }

        public static Direction ToDirection(this Vector2Int dir)
        {
            if (dir.x * dir.y != 0 || (dir.x == 0 && dir.y == 0))
                MonoBehaviour.print($"direction not supported: {dir}");
            if (dir.x != 0) return dir.x > 0 ? East : West;
            return dir.y > 0 ? North : South;
        }

        public static Vector2Int ToVector(this Direction dir)
        {
            return dir switch
            {
                East => Vector2Int.right,
                West => Vector2Int.left,
                North => Vector2Int.up,
                South => Vector2Int.down,
                _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
            };
        }
    }
}