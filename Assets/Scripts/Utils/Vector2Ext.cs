using UnityEngine;

namespace Utils
{
    public static class Vector2Ext
    {
        public static Vector2 Rotate(Vector2 v, float degrees) {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
         
            float tx = v.x;
            float ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }

        public static Vector2 GetPerpendicular(Vector2 v, bool clockwise = true)
        {
            return (clockwise ? 1 : -1) * new Vector2(v.y, -v.x);
        }

        public static Vector2 GetProjection(Vector2 v, Vector2 projectOn)
        {
            return (Vector2.Dot(v, projectOn) / projectOn.sqrMagnitude) * projectOn;
        }

        /*
         * for v = a*axis + per where <axis, per>=0, return per 
         */
        public static Vector2 GetPerpendicularPortion(Vector2 v, Vector2 axis)
        {
            return v - GetProjection(v, axis);
        }
        
        /*
         * for v = a*axis + per where <axis, per>=0, return a*axis
         */
        public static Vector2 GetAxisPortion(Vector2 v, Vector2 axis)
        {
            return GetProjection(v, axis);
        }

        // https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection
        public static bool Intersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            (float x1, float x2, float x3, float x4) = (p1.x, p2.x, p3.x, p4.x);
            (float y1, float y2, float y3, float y4) = (p1.y, p2.y, p3.y, p4.y);

            var t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) /
                    ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));
            var u = ((x1 - x3) * (y1 - y2) - (y1 - y3) * (x1 - x2)) /
                    ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));

            return t is >= 0 and <= 1 && u is >= 0 and <= 1;
        }
    }
}

