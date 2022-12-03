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

    #endregion

    #region Non-Serialized Fields

    private Vector2 _aimDirection;

    private Yoyo _yoyo;

    private bool _precisioning;

    #endregion

    #region Properties

    private bool Aiming => _aimLine.gameObject.activeSelf;

    #endregion

    #region Function Events

    #endregion

    #region Input Actions

    public void OnAim(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                _aimDirection = context.ReadValue<Vector2>();
                break;
            case InputActionPhase.Canceled:
                _aimDirection = Vector2.zero;
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
                    _yoyo.Shoot(_aimDirection);
                }

                break;
        }
    }

    public void OnPresicionShot(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                _yoyo.PrecisionShoot();
                DelayInvoke(
                    () => {_yoyo.CancelPrecision();}, _precisionTime);
                break;
            case InputActionPhase.Canceled:
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
        if (_yoyo.State == Yoyo.YoyoState.PERCISION)
        {
            _yoyo.PrecisionDirection = _aimDirection;
        }
        else
        {
            if (_aimDirection != Vector2.zero)
            {
                var zRotation = Vector3.SignedAngle(_aimDirection, Vector3.up, -Vector3.forward);
                var q = Quaternion.Euler(0, 0, zRotation);
                if (!Aiming)
                {
                    _aimPivot.transform.rotation = q;
                    _aimLine.SetActive(true);
                }
                else
                {
                    _aimPivot.transform.rotation =
                        Quaternion.Slerp(_aimPivot.transform.rotation, q, Time.deltaTime * _rotationSpeed);
                }
            }
        }
    }

    #endregion
}