using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerController : MonoBehaviourExt, CharacterMap.IPlayerActions
{
    #region Serialized Fields

    [Header("Movement")] [SerializeField] private float _speed = 2;
    [SerializeField] private float _maxSpeed = 2;
    [SerializeField] private float _acceleration = 2;
    [SerializeField] private float _deceleration = 2;

    [Header("Dash")] [SerializeField] private float _dashTime;
    [SerializeField] private float _dashBonus;
    [SerializeField] private float _dashCooldown;

    #endregion

    #region Non-Serialized Fields

    private Vector3 _dashDirection; // used so we can keep tracking the input direction without changing dash direction
    private bool _canDash = true;
    private bool _dashing;

    private Rigidbody2D _rigidbody;
    private Vector2 _direction;

    #endregion

    #region Properties

    private Vector2 DesiredVelocity => _direction * _speed;
    private float DashSpeed => _maxSpeed + _dashBonus;

    #endregion

    #region Function Events

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _yoyo = GetComponentInChildren<Yoyo>();
    }

    protected override void Update()
    {
        base.Update();

        SetAim(); // Shooting Controller
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        MoveCharacter();
        ModifyPhysics();
    }

    private void OnEnable()
    {
        GameManager.PlayerControllerEnabled = true;
    }

    private void OnDisable()
    {
        GameManager.PlayerControllerEnabled = false;
    }

    #endregion

    #region ActionMap

    // =============================== Action Map ======================================================================

    public void OnMove(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                _direction = context.ReadValue<Vector2>();
                break;
            case InputActionPhase.Canceled:
                _direction = Vector2.zero;
                break;
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                if (!_canDash || _dashing) return;
                _dashDirection = _direction.normalized;
                _dashing = true;
                _canDash = false;
                DelayInvoke(
                    () => { _canDash = true; }, _dashCooldown);
                DelayInvoke(
                    () => { _dashing = false; }, _dashTime);
                break;
        }
    }

    #endregion

    #region Public Methods

    #endregion

    #region Private Methods

    private void MoveCharacter()
    {
        if (_dashing)
        {
            _rigidbody.velocity = _dashDirection * DashSpeed;
            return;
        }

        _rigidbody.velocity = Vector2.Lerp(_rigidbody.velocity,
            DesiredVelocity,
            _acceleration * Time.fixedDeltaTime);

        if (DesiredVelocity.magnitude > _maxSpeed)
        {
            _rigidbody.velocity = DesiredVelocity / DesiredVelocity.magnitude * _maxSpeed;
        }
    }

    private void ModifyPhysics()
    {
        var changingDirection = Vector3.Angle(_direction, _rigidbody.velocity) >= 90;

        // Make "linear drag" when changing direction
        if (changingDirection)
        {
            _rigidbody.velocity = Vector2.Lerp(
                _rigidbody.velocity,
                Vector2.zero,
                _deceleration * Time.fixedDeltaTime);
        }

        if (_direction.magnitude == 0 && _rigidbody.velocity.magnitude < 0.2f)
        {
            _rigidbody.velocity *= Vector2.zero;
        }
    }

    #endregion
}