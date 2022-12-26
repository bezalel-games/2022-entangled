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
    }
}

