using System;
using System.Collections;
using Managers;
using HP_System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public partial class PlayerController : YoyoOwner, CharacterMap.IPlayerActions
    {
        #region Serialized Fields

        [Header("Movement")] [SerializeField] private float _speed = 2;
        [SerializeField] private float _maxSpeed = 2;
        [SerializeField] private float _acceleration = 2;
        [SerializeField] private float _deceleration = 2;

        [Header("Dash")] 
        [field: SerializeField] public float DashTime;
        [SerializeField] private float _dashBonus;
        [SerializeField] private float _dashCooldown;

        [Header("Other Stats")]
        [SerializeField] private float _mpRecoveryOnAttack;
        [SerializeField][Range(0f,1f)][Tooltip("In percentages of maxMP")] private float _mpRecoveryPerSec; 
        [SerializeField] private float _mpRecoveryOnHit;
        [SerializeField] private float _mpPrecisionReduction;

        #endregion

        #region Non-Serialized Fields

        private Vector3
            _dashDirection; // used so we can keep tracking the input direction without changing dash direction

        private bool _canDash = true;
        private bool _dashing;
        
        private Vector2 _direction;
        private bool _invulnerable;

        private bool _overridenMovement; // true if the movement is currently not controlled by player input

        private const float DEC_THRESHOLD = 0.2f;

        private int DashLayer;
        private int PlayerLayer;

        #endregion

        #region Properties

        public float MpRecovery
        {
            get => _mpRecoveryPerSec;
            set => _mpRecoveryPerSec = value;
        }

        protected override bool Invulnerable
        {
            get => _invulnerable;
            set
            {
                _invulnerable = value;
                gameObject.layer = value ? DashLayer : PlayerLayer;
            }
        }

        private Vector2 DesiredVelocity => _direction * _speed;
        private float DashSpeed => _maxSpeed + _dashBonus;

        private Vector2 Direction
        {
            get => _direction;
            set
            {
                _direction = value.normalized;
                if (!_dashing)
                {
                    Animator.SetFloat(xDirection, Mathf.Abs(_direction.x) <= 0.1f ? 0 : _direction.x );
                    Animator.SetFloat(yDirection, Mathf.Abs(_direction.y) <= 0.1f ? 0 : _direction.y );
                }
            }
        }

        #endregion

        #region C# Events

        public event Action DashStartEvent;
        public Action InteractEvent;

        #endregion

        #region Function Events

        protected override void Awake()
        {
            base.Awake();
            Init();
        }

        protected override void Update()
        {
            base.Update();

            if(IsDead) return;
            
            if(Yoyo != null && Yoyo.State != Yoyo.YoyoState.PRECISION)
                Mp += MaxMp * _mpRecoveryPerSec * Time.deltaTime;

            SetAim();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if(IsDead) return;
            
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
            if (_overridenMovement) return;
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    Direction = context.ReadValue<Vector2>();
                    Animator.SetBool(Walking, true);
                    break;
                case InputActionPhase.Canceled:
                    Direction = Vector2.zero;
                    Animator.SetBool(Walking, false);
                    break;
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    InteractEvent?.Invoke();
                    InteractEvent = null;
                    break;
            }
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    DashStartEvent?.Invoke();
                    if (!_canDash || _dashing || IsDead) return;
                    
                    _dashDirection = _direction.normalized;
                    
                    _dashing = true;
                    _canDash = false;
                    Invulnerable = true;
                    Animator.SetTrigger(Dash);

                    DelayInvoke(() => { _canDash = true; }, _dashCooldown);
                    
                    DelayInvoke(() =>
                    {
                        _dashing = false;
                        Invulnerable = false;
                    }, DashTime);
                    break;
            }
        }

        public void OnMinimap(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    CameraManager.ToggleMinimap(true);
                    break;
                case InputActionPhase.Canceled:
                    CameraManager.ToggleMinimap(false);
                    break;
            }
        }

        #endregion

        #region Public Methods

        public void AfterDeathAnimation()
        {
            GameManager.PlayerKilled();
        }

        public void OverrideMovement(Vector3 dir, float threshold)
        {
            _overridenMovement = true;
            var velocity = Rigidbody.velocity;
            if (velocity.x * dir.x <= 0)
                velocity.x = 0;
            if (velocity.y * dir.y <= 0)
                velocity.y = 0;
            Rigidbody.velocity = velocity;
            Direction = dir;
            Predicate<Vector3> passThresholdTest;
            if (dir.x != 0)
                passThresholdTest = dir.x > 0 ? (pos) => pos.x > threshold : (pos) => pos.x < threshold;
            else
                passThresholdTest = dir.y > 0 ? (pos) => pos.y > threshold : (pos) => pos.y < threshold;
            StartCoroutine(StopOverrideOnThresholdPass(passThresholdTest));
        }

        public override void OnSuccessfulHit()
        {
            if (Yoyo.State != Yoyo.YoyoState.PRECISION)
                Mp += _mpRecoveryOnAttack;
        }

        public override void OnPrecision()
        {
            Mp -= _mpPrecisionReduction * Time.unscaledDeltaTime;
            if (Mp <= 0)
            {
                Yoyo.CancelPrecision();
            }
        }

        #endregion

        #region Private Methods

        private void Init()
        {
            Yoyo = GetComponentInChildren<Yoyo>();

            PlayerLayer = LayerMask.NameToLayer("Player");
            DashLayer = LayerMask.NameToLayer("Dash");

            switch (LoadManager.CurrentScene)
            {
                case LoadManager.Scene.RUN:
                    StartRun();
                    break;
            }
        }
        
        private  void StartRun()
        {
            Hp = MaxHp;
            Mp = MaxMp;
            DashStartEvent = null;
            
            // enter "animation"
            OverrideMovement(Vector3.up, 0);
        }
        
        private void MoveCharacter()
        {
            if (_dashing)
            {
                Rigidbody.velocity = _dashDirection * DashSpeed;
                return;
            }

            if (PushbackVector != Vector2.zero)
            {
                Rigidbody.velocity = PushbackVector;
            }
            else
            {
                Rigidbody.velocity = Vector2.Lerp(Rigidbody.velocity,
                    DesiredVelocity,
                    _acceleration * Time.fixedDeltaTime);

                if (DesiredVelocity.magnitude > _maxSpeed)
                {
                    Rigidbody.velocity = DesiredVelocity.normalized * _maxSpeed;
                }
                
            }
        }

        private void ModifyPhysics()
        {
            var changingDirection = Vector3.Angle(_direction, Rigidbody.velocity) >= 90;

            // Make "linear drag" when changing direction
            if (changingDirection)
            {
                Rigidbody.velocity = Vector2.Lerp(
                    Rigidbody.velocity,
                    Vector2.zero,
                    _deceleration * Time.fixedDeltaTime);
            }

            if (_direction.magnitude == 0 && Rigidbody.velocity.magnitude < DEC_THRESHOLD)
            {
                Rigidbody.velocity *= Vector2.zero;
            }
        }

        private IEnumerator StopOverrideOnThresholdPass(Predicate<Vector3> passedThreshold)
        {
            while (!passedThreshold.Invoke(transform.position))
            {
                yield return null;
            }

            Direction = Vector2.zero;
            _overridenMovement = false;
        }

        #endregion

        #region IHittable

        public override void OnDie()
        {
            Rigidbody.bodyType = RigidbodyType2D.Static;
            Animator.SetTrigger("Dead");
        }

        public override void OnHit(Transform attacker, float damage, bool pushBack = true)
        {
            if (Invulnerable) return;

            base.OnHit(attacker, damage, pushBack);
            Mp += _mpRecoveryOnHit;

            if (!attacker)
                return;
            
            Invulnerable = true;
            DelayInvoke((() => { Invulnerable = false; }), _pushbackTime);
        }

        #endregion
    }
}