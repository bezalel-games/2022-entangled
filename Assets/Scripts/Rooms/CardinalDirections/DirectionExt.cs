using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Rooms.CardinalDirections.Direction;
using Random = System.Random;

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

        public static bool IsHorizontal(this Direction dir)
        {
            return (byte)dir % 2 == 0;
        }

        public static Direction ToDirection(this Vector2Int dir)
        {
            if (dir.x * dir.y != 0 || (dir.x == 0 && dir.y == 0))
                MonoBehaviour.print($"direction not supported: {dir}");
            if (dir.x != 0) return dir.x > 0 ? EAST : WEST;
            return dir.y > 0 ? NORTH : SOUTH;
        }

        public static Direction ToDirectionRounded(this Vector2Int dir)
        {
            var absX = Math.Abs(dir.x);
            var absY = Math.Abs(dir.y);

            if (absX >= absY)
                return dir.x >= 0 ? EAST : WEST;
            return dir.y >= 0 ? NORTH : SOUTH;
        }

        public static Vector2Int ToVector(this Direction dir)
        {
            return dir switch
            {
                EAST => Vector2Int.right,
                WEST => Vector2Int.left,
                NORTH => Vector2Int.up,
                SOUTH => Vector2Int.down,
                _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
            };
        }
    }
}