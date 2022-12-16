using System;
using Managers;
using Enemies;
using HP_System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public partial class PlayerController : LivingBehaviour, CharacterMap.IPlayerActions
    {
        #region Serialized Fields

        [Header("Movement")] [SerializeField] private float _speed = 2;
        [SerializeField] private float _maxSpeed = 2;
        [SerializeField] private float _acceleration = 2;
        [SerializeField] private float _deceleration = 2;

        [Header("Dash")] [SerializeField] private float _dashTime;
        [SerializeField] private float _dashBonus;
        [SerializeField] private float _dashCooldown;

        [Header("Other Stats")] 
        [SerializeField] private float _mpRecoveryOnAttack;
        [SerializeField] private float _mpRecoveryOnHit;
        [SerializeField] private float _mpPrecisionReduction;

        #endregion

        #region Non-Serialized Fields

        private Vector3
            _dashDirection; // used so we can keep tracking the input direction without changing dash direction

        private bool _canDash = true;
        private bool _dashing;

        private Rigidbody2D _rigidbody;
        private Vector2 _direction;

        private const float DEC_THRESHOLD = 0.2f;

        #endregion

        #region Properties

        private Vector2 DesiredVelocity => _direction * _speed;
        private float DashSpeed => _maxSpeed + _dashBonus;

        #endregion

        #region C# Events

        public event Action DashStartEvent;

        #endregion

        #region Function Events

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            Yoyo = GetComponentInChildren<Yoyo>();
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

        protected override void OnEnable()
        {
            base.OnEnable();
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
                    DashStartEvent?.Invoke();
                    if (!_canDash || _dashing) return;
                    _dashDirection = _direction.normalized;
                    _dashing = true;
                    _canDash = false;
                    Invulnerable = true;
                    DelayInvoke( () => { _canDash = true; }, _dashCooldown);
                    DelayInvoke( () => { _dashing = false; Invulnerable = false; }, _dashTime);
                    break;
            }
        }

        #endregion

        #region Public Methods

        public void OnHitEnemy(Enemy enemy)
        {
            if(Yoyo.State != Yoyo.YoyoState.PRECISION)
                Mp += _mpRecoveryOnAttack;
        }

        public void OnPrecision()
        {
            Mp -= _mpPrecisionReduction * Time.unscaledDeltaTime;
            if (Mp <= 0)
            {
                Yoyo.CancelPrecision();
            }
        }

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
                _rigidbody.velocity = DesiredVelocity.normalized * _maxSpeed;
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

            if (_direction.magnitude == 0 && _rigidbody.velocity.magnitude < DEC_THRESHOLD)
            {
                _rigidbody.velocity *= Vector2.zero;
            }
        }

        #endregion

        #region IHittable

        public override void OnDie()
        {
            print("Dead");
        }

        public override void OnHit(float damage)
        {
            if(Invulnerable) return;
            
            base.OnHit(damage);
            Mp += _mpRecoveryOnHit;
        }

        #endregion
    }
}