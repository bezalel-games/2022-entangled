using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Player
{
    public class Line : MonoBehaviour
    {
        #region Serialized Fields

        #endregion

        #region Non-Serialized Fields

        private LineRenderer _renderer;
        private List<Vector2> points = new();
        private EdgeCollider2D _collider;

        #endregion

        #region Properties

        public Vector2 CurrentPosition => _renderer != null && _renderer.positionCount > 0
            ? _renderer.GetPosition(_renderer.positionCount - 1)
            : transform.position;

        #endregion

        #region Function Events

        private void Start()
        {
            _renderer = GetComponent<LineRenderer>();
            _collider = GetComponent<EdgeCollider2D>();
        }

        #endregion

        #region Public Methods

        public void AddPosition(Vector2 pos)
        {
            if (points.Count == 0)
            {
                _renderer.transform.position = pos;
                _collider.offset = -pos;
            }
            points.Add(pos);
            _collider.SetPoints(points);
            _renderer.SetPosition(_renderer.positionCount++, pos);
        }

        #endregion

        #region Private Methods

        private void CheckCollision(Vector2 pos)
        {
            
            
        }

        #endregion
    }
}