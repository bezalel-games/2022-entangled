using UnityEngine;

namespace Player.Yoyo
{
    public class Line : MonoBehaviour
    {
        #region Serialized Fields

        #endregion

        #region Non-Serialized Fields

        private LineRenderer _renderer;

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
        }

        #endregion

        #region Public Methods

        public void AddPosition(Vector2 pos)
        {
            _renderer.SetPosition(_renderer.positionCount++, pos);
        }

        #endregion

        #region Private Methods

        #endregion
    }
}