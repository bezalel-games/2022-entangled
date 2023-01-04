using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Utils;

namespace Player
{
    public class Line : MonoBehaviour
    {
        #region Serialized Fields

        #endregion

        #region Non-Serialized Fields

        private LineRenderer _renderer;
        private List<Vector2> points = new();
        // private EdgeCollider2D _collider;
        private PolygonCollider2D _polygonCollider;

        private static int Mask = -1;

        #endregion

        #region Properties

        public Vector2 CurrentPosition => _renderer != null && _renderer.positionCount > 0
            ? _renderer.GetPosition(_renderer.positionCount - 1)
            : transform.position;
        
        public float EnemyFreezeTime { get; set; }

        #endregion

        #region Function Events

        private void Start()
        {
            _renderer = GetComponent<LineRenderer>();
            // _collider = GetComponent<EdgeCollider2D>();
            _polygonCollider = GetComponent<PolygonCollider2D>();
            if(Mask != -1)
                Mask = LayerMask.GetMask("YoyoLine");
        }

        #endregion

        #region Public Methods

        public void AddPosition(Vector2 pos)
        {
            if (_polygonCollider.points.Length > 0)
            {
                _polygonCollider.points = null;
            }
            
            if (points.Count == 0)
            {
                _renderer.transform.position = pos;
                _polygonCollider.offset = -pos;
                // _collider.offset = -pos;
            }
            
            points.Add(pos);
            _renderer.SetPosition(_renderer.positionCount++, pos);
            int start = CheckCollision();
            if (start >= 0)
            {
                _polygonCollider.points = points.GetRange(start, points.Count - start).ToArray();
            }
        }

        #endregion

        #region Private Methods

        /*
         * returns i such that pos is on the line between points[i] and points[i+1]
         * return -1 if no collision
         */
        private int CheckCollision()
        {
            // var hit = Physics2D.Linecast(points[^1], pos, LayerMask.GetMask("YoyoLine"));
            // if (hit.collider == null)
            //     return -1;
            //
            // var hitPos = hit.collider.transform.position;
            // var edgePos = _collider.ClosestPoint(hitPos);
            //
            // int index = points.IndexOf(edgePos);
            // print($"Collision? {index != -1}.");
            // print($"Collision at {hitPos}. CollisionPoint {edgePos} at index: {index}");
            // if (index == 0)
            //     return 0;
            // if (index == points.Count - 1)
            //     return index - 1;
            // return Vector2.Distance(edgePos, points[index - 1]) < Vector2.Distance(edgePos, points[index + 1])
            //     ? index - 1
            //     : index;
            
            if (points.Count < 2)
                return -1;
            
            for (int i = 0; i < points.Count - 3; i++)
            {
                if (Vector2Ext.Intersect(points[i], points[i + 1], points[^2], points[^1]))
                {
                    print($"Collision: {i},{i+1} and {points.Count-2},{points.Count-1}");
                    return i;
                }
            }

            return -1;
        }

        #endregion
    }
}