using Enemies;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Player
{
    public partial class PlayerController
    {
        #region Serialized Fields

        [Header("Shooting")] [SerializeField] private float _rotationSpeed;
        [SerializeField] private GameObject _aimLine;
        [SerializeField] private GameObject _aimPivot;
        [SerializeField] private float _aimAssistRange;

        #endregion

        #region Non-Serialized Fields

        private const float ANGLE_THRESHOLD = 0.2f;
    
        private Vector2 _aimDirection;

        private bool _aiming;

        private Yoyo _yoyo;

        private bool _precisioning;

        #endregion

        #region Properties

        #endregion

        #region Function Events

        private void OnDrawGizmos()
        {
            DrawAimAssist();
        }

        #endregion

        #region Input Actions

        public void OnAim(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    _aiming = true;
                    _aimDirection = context.ReadValue<Vector2>();
                    break;
                case InputActionPhase.Canceled:
                    _aiming = false;
                    _aimLine.SetActive(false);
                    break;
            }
        }

        public void OnShoot(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    if (_yoyo.State == Yoyo.YoyoState.IDLE)
                    {
                        var desiredDir = (!_aiming && _direction != Vector2.zero)
                            ? _direction
                            : _aimDirection;
                        desiredDir = AimAssist(desiredDir);
                        SetPivotRotation(desiredDir);
                        _yoyo.Shoot(desiredDir);
                    }

                    break;
            }
        }

        public void OnPrecisionShot(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    if (_yoyo.State != Yoyo.YoyoState.IDLE) return;
                    _rigidbody.velocity = Vector2.zero;
                    _yoyo.PrecisionShoot();
                    break;
                case InputActionPhase.Canceled:
                    if (_yoyo.State != Yoyo.YoyoState.PRECISION) return;

                    _yoyo.CancelPrecision();
                    break;
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        private Vector2 AimAssist(Vector2 desiredDir)
        {
            var collider = RayCastEnemy(desiredDir);
            if (collider != null)
            {
                return desiredDir;
            }

            var rightRotation = Vector2.zero;
            var leftRotation = Vector2.zero;
        
            for (float angle = ANGLE_THRESHOLD; angle < _aimAssistRange; angle += ANGLE_THRESHOLD)
            {
                rightRotation = Vector2Ext.Rotate(desiredDir, angle);
                collider = RayCastEnemy(rightRotation);
                if (collider != null)
                {
                    return rightRotation;
                }

                leftRotation = Vector2Ext.Rotate(desiredDir, -angle);
                collider = RayCastEnemy(leftRotation);
                if (collider != null)
                {
                    return leftRotation;
                }
            }

            return desiredDir;
        }

        private void SetAim()
        {
            if (_yoyo.State == Yoyo.YoyoState.PRECISION)
            {
                var currDir = _yoyo.PrecisionDirection;
                _yoyo.PrecisionDirection = _aimDirection;

                SetPivotRotation(_aimDirection);
            }
            else
            {
                bool immediateRotation = false;
            
                if (_aiming && !_aimLine.activeSelf)
                {
                    _aimLine.SetActive(true);
                    immediateRotation = true;
                }
            
                SetPivotRotation(_aimDirection, immediateRotation);
            }
        }

        private void SetPivotRotation(Vector3 euler, bool immediate=true)
        {
            var zRotation = Vector3.SignedAngle(euler, Vector3.up, -Vector3.forward);
            var q = Quaternion.Euler(0, 0, zRotation);
        
            float t = immediate ? 1 : Time.deltaTime * _rotationSpeed;
        
            _aimPivot.transform.rotation =
                Quaternion.Slerp(_aimPivot.transform.rotation, q, t);
        }

        private Collider2D RayCastEnemy(Vector2 direction)
        {
            var enemy = Physics2D.Raycast(transform.position, direction, 10, layerMask: Enemy.Layer).collider;
            if (enemy != null)
            {
                // when add walls layer, use this
            
                // var hit = Physics2D.Raycast(transform.position, direction, 10).collider;
                // print($"{hit.gameObject.tag},{enemy.gameObject.tag}");
                // if (hit.GetInstanceID() == enemy.GetInstanceID())
                // {
                //     return hit;
                // }
            
                return enemy;
            }

            return null;
        }

        private void DrawAimAssist()
        {
            var position = transform.position;
            var rot = _aimDirection;

            Gizmos.DrawLine(position, position + (Vector3) Vector2Ext.Rotate(rot, _aimAssistRange).normalized * 10);
            Gizmos.DrawLine(position, position + (Vector3) Vector2Ext.Rotate(rot, -_aimAssistRange).normalized * 10);
        }

        #endregion
    }
}