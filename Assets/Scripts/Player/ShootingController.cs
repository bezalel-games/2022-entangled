using System;
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

        private bool _precisioning;

        #endregion

        #region Properties

        public Yoyo Yoyo { get; private set; }

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
            if(Yoyo.enabled == false) return;
            
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    _aiming = true;
                    _aimDirection = context.ReadValue<Vector2>();
                    break;
                case InputActionPhase.Canceled:
                    _aiming = false;
                    _aimLine.SetActive(false);
                    Yoyo.QuickShotDirection = Vector2.zero;
                    break;
            }
        }

        public void OnShoot(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    if (Yoyo.State == Yoyo.YoyoState.IDLE)
                    {
                        var desiredDir = (!_aiming && _direction != Vector2.zero)
                            ? _direction
                            : _aimDirection;
                        desiredDir = AimAssist(desiredDir);
                        SetPivotRotation(desiredDir);
                        Yoyo.Shoot(desiredDir, _direction);
                    }

                    break;
            }
        }

        public void OnPrecisionShot(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    if (Yoyo.State != Yoyo.YoyoState.IDLE) return;
                    if(Mp > 0)
                        Yoyo.PrecisionShoot();
                    break;
                case InputActionPhase.Canceled:
                    if (Yoyo.State != Yoyo.YoyoState.PRECISION) return;
                    Yoyo.CancelPrecision();
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
            if(Yoyo == null) return;
            
            if (Yoyo.State == Yoyo.YoyoState.PRECISION)
            {
                var currDir = Yoyo.PrecisionDirection;
                Yoyo.PrecisionDirection = _aimDirection;

                SetPivotRotation(_aimDirection);
                return;
            }

            if (_aiming && Yoyo.State is Yoyo.YoyoState.SHOOT or Yoyo.YoyoState.BACK)
            {
                Yoyo.QuickShotDirection = _aimDirection;
            }

            bool immediateRotation = false;

            if (_aiming && !_aimLine.activeSelf)
            {
                _aimLine.SetActive(true);
                immediateRotation = true;
            }

            SetPivotRotation(_aimDirection, immediateRotation);
        }

        private void SetPivotRotation(Vector3 euler, bool immediate = true)
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