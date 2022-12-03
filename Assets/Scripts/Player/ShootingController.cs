using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public partial class PlayerController
{
    #region Serialized Fields

    [Header("Shooting")] [SerializeField] private float _rotationSpeed;
    [SerializeField] private GameObject _aimLine;
    [SerializeField] private GameObject _aimPivot;

    [Header("Precision Shot")] [SerializeField]
    private float _precisionTime;

    [SerializeField] private float _precisionRotationSpeed;

    #endregion

    #region Non-Serialized Fields

    private Vector2 _aimDirection;

    private bool _aiming;

    private Yoyo _yoyo;

    private bool _precisioning;

    #endregion

    #region Properties

    #endregion

    #region Function Events

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
                    SetPivotRotation(desiredDir);
                    _yoyo.Shoot(desiredDir);
                }

                break;
        }
    }

    public void OnPresicionShot(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                if (_yoyo.State != Yoyo.YoyoState.IDLE) return;
                _rigidbody.velocity = Vector2.zero;
                _yoyo.PrecisionShoot();
                DelayInvoke(
                    () => { _yoyo.CancelPrecision(); }, _precisionTime);
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

    private void SetAim()
    {
        if (_yoyo.State == Yoyo.YoyoState.PRECISION)
        {
            var currDir = _yoyo.PrecisionDirection;
            _yoyo.PrecisionDirection = Vector2.Lerp(currDir, _aimDirection, _precisionRotationSpeed * Time.deltaTime);

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

    #endregion
}