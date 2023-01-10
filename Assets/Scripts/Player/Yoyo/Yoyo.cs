using System;
using Cards.Buffs.ActiveBuffs;
using Enemies;
using HP_System;
using Managers;
using UnityEngine;
using Utils;

namespace Player
{
    public class Yoyo : MonoBehaviourExt
    {
        public enum YoyoState
        {
            IDLE,
            SHOOT,
            BACK,
            PRECISION
        }

        #region Serialized Fields

        [Header("Quick Shot")]
        [SerializeField] private float _shootSpeed;

        [SerializeField] private float _maxDistance;

        [SerializeField] [Range(0f, 1f)]
        [Tooltip("Portion of \"Max Distance\" in which the weapon should slow down on quickshot")]
        private float _easeDistance;

        [SerializeField]
        [Tooltip("The lower this is, the slower the weapon will become before going back on quick shot")]
        private float _easeVelocityThreshold = 1.5f;

        [SerializeField] private float _backSpeed;

        [SerializeField] [Tooltip("The time it will take the weapon to reach \"Back Speed\" when going back")]
        private float _backEaseDuration;

        [SerializeField]
        [Tooltip(
            "The higher this is, the more turning the right stick during quickshots will effect the direction of the weapon")]
        private float _turnFactor;

        [Header("Precision Shot")]
        [SerializeField] private float _precisionRotationSpeed;

        [SerializeField] private float _precisionSpeed;
        [SerializeField] private float _resolution;
        [SerializeField] private Line _linePrefab;
        [SerializeField] private float _timeScale;

        [Header("Other")] [SerializeField] private float _waitForReturn;

        [SerializeField] private Transform _parent;
        [SerializeField] private Transform _initPos;

        #endregion

        #region Non-Serialized Fields

        private Rigidbody2D _rigidbody;
        private Collider2D _collider;

        private Vector3 _direction;
        private Vector2 _precisionDirection;
        private Vector2 _quickShotLastPos;
        private Vector2 _quickShotDirection;
        private float _quickShotCumDistance;
        private float _actualMaxDistance;

        private YoyoState _state = YoyoState.IDLE;
        private YoyoOwner _owner;

        private Line _currentLine;

        private float _stopBackEase;

        #endregion

        #region C# Events

        public event Action ReachedThrowPeak;
        public event Action FinishedPrecision;

        #endregion

        #region Properties

        [field: SerializeField] public float Damage { get; private set; }

        public Line LinePrefab => _linePrefab;

        private Vector2 BackDirection =>
            ((Vector2)_parent.transform.position - (Vector2)transform.position).normalized;

        public YoyoState State
        {
            get => _state;
            private set
            {
                if (_state != value && value == YoyoState.BACK)
                    ReachedThrowPeak?.Invoke();
                _state = value;
            }
        }

        public Vector2 PrecisionDirection
        {
            get => _precisionDirection;
            set => _precisionDirection =
                Vector2.Lerp(_precisionDirection, value, _precisionRotationSpeed * Time.unscaledTime);
        }

        public Vector2 QuickShotDirection
        {
            get => _quickShotDirection;
            set => _quickShotDirection = value.normalized;
        }

        public float Size
        {
            get => _collider.transform.localScale.x;
            set => _collider.transform.localScale = Vector3.one * value;
        }

        public ExplosiveYoyo ExplosiveYoyo { get; set; }

        #endregion

        #region Function Events

        private void Start()
        {
            _owner = GetComponentInParent<YoyoOwner>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _collider = GetComponentInChildren<Collider2D>();

            _linePrefab.Damage = 0;
            _linePrefab.StayTime = 0;
        }

        protected override void Update()
        {
            base.Update();
            transform.rotation = Quaternion.identity;

            switch (State)
            {
                case YoyoState.IDLE:
                    if (transform.position != _initPos.position)
                        transform.position = _initPos.position;
                    break;
                case YoyoState.PRECISION:
                    DrawPath();
                    break;
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (State != YoyoState.IDLE)
            {
                MoveYoyo();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (IsOwner(other))
            {
                if (State == YoyoState.BACK)
                    Reset();
                return;
            }

            switch (State)
            {
                case YoyoState.BACK:
                case YoyoState.PRECISION:
                    OnHitEnemy(other.GetComponent<IHittable>());
                    break;
                case YoyoState.SHOOT:
                    OnHitEnemy(other.GetComponent<IHittable>());
                    GoBack(true);
                    break;
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (IsOwner(other))
            {
                if (State == YoyoState.BACK)
                    Reset();
                return;
            }

            if (State == YoyoState.SHOOT)
            {
                OnHitEnemy(other.GetComponent<IHittable>());
                GoBack();
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            transform.rotation = Quaternion.identity;
        }
#endif

        #endregion

        #region Public Methods

        public void Shoot(Vector3 aimDirection, Vector3 moveDirection)
        {
            if (aimDirection != Vector3.zero)
            {
                aimDirection = aimDirection.normalized;
                moveDirection = moveDirection.normalized;

                var movePortionOnAim = Vector2Ext.GetAxisPortion(moveDirection, aimDirection);
                var maxDistanceFactor =
                    movePortionOnAim.magnitude * Mathf.Sign(Vector2.Dot(aimDirection, moveDirection));

                if (Mathf.Abs(maxDistanceFactor) > 1)
                    maxDistanceFactor = Mathf.Sign(maxDistanceFactor);
                _actualMaxDistance = _maxDistance * (1 + 0.5f * maxDistanceFactor);

                State = YoyoState.SHOOT;
                _direction = aimDirection;
                _rigidbody.velocity = aimDirection;
                _quickShotLastPos = transform.position;
                transform.SetParent(null);
            }
        }

        public void PrecisionShoot()
        {
            State = YoyoState.PRECISION;

            if (_currentLine != null)
            {
                RemovePath(_currentLine);
            }

            GameManager.ScaleTime(_timeScale);

            transform.SetParent(null);

            _currentLine = Instantiate(_linePrefab, transform.position, Quaternion.identity);
            _currentLine.Damage = _linePrefab.Damage;
            _currentLine.StayTime = _linePrefab.StayTime;
        }

        public void CancelPrecision()
        {
            if (_currentLine == null) return;

            GameManager.ScaleTime(1);
            if (FinishedPrecision != null)
            {
                FinishedPrecision.Invoke();
            }

            if (_currentLine.StayTime > 0)
            {
                Line l = _currentLine;
                DelayInvoke((() => { RemovePath(l); }), _currentLine.StayTime);

                _currentLine = null;
            }
            else
            {
                RemovePath(_currentLine);
            }

            GoBack();
        }

        public void LeaveTrail()
        {
            if (_currentLine == null) return;
            _currentLine.CreateCollider();
        }

        #endregion

        #region Private Methods

        private void OnHitEnemy(IHittable hittableObj)
        {
            if (hittableObj is null or Enemy { HasBarrier: true }) return;
            hittableObj.OnHit(transform, Damage, State != YoyoState.PRECISION);
            _owner.OnSuccessfulHit();
        }

        private bool IsOwner(Component other) => other.GetComponent<YoyoOwner>() == _owner;

        private void MoveYoyo()
        {
            Vector2 vel;
            Vector2 perpendicular;
            Vector2 velWithPerpendicular;

            switch (State)
            {
                case YoyoState.SHOOT:

                    var position = transform.position;

                    _quickShotCumDistance += Vector2.Distance(position, _quickShotLastPos);
                    _quickShotLastPos = position;

                    vel = _rigidbody.velocity;
                    perpendicular = Vector2Ext.GetPerpendicularPortion(QuickShotDirection, vel);
                    velWithPerpendicular = (vel + _turnFactor * perpendicular).normalized;

                    if (_quickShotCumDistance < _easeDistance * _actualMaxDistance)
                    {
                        _rigidbody.velocity = velWithPerpendicular * _shootSpeed;
                    }
                    else
                    {
                        /*
                         * for d = cumulativeDistance, e = easeDistance, m = maxDistance
                         * if d > em -> d = e*m + (m - e*m)*t -> t = (d - e*m)/(m - e*m)
                         */
                        var t = (_quickShotCumDistance - _easeDistance * _actualMaxDistance) /
                                (_actualMaxDistance - _easeDistance * _actualMaxDistance);
                        _rigidbody.velocity = Vector2.Lerp(velWithPerpendicular * vel.magnitude, Vector2.zero, t);
                    }

                    if (vel.magnitude <= _easeVelocityThreshold)
                        GoBack();

                    break;

                case YoyoState.PRECISION:
                    _rigidbody.velocity = PrecisionDirection.normalized * (_precisionSpeed / _timeScale);
                    _owner.OnPrecision();
                    break;

                case YoyoState.BACK:
                    vel = BackDirection * _backSpeed;
                    perpendicular = Vector2Ext.GetPerpendicularPortion(QuickShotDirection, -vel);
                    velWithPerpendicular = (vel + _turnFactor * perpendicular).normalized;

                    if (Time.time > _stopBackEase)
                    {
                        _rigidbody.velocity = velWithPerpendicular * _backSpeed;
                    }
                    else
                    {
                        var backT = 1 - ((_stopBackEase - Time.time) / _backEaseDuration);
                        _rigidbody.velocity = Vector2.Lerp(_rigidbody.velocity,
                            velWithPerpendicular * vel.magnitude, backT);
                    }

                    break;
            }
        }

        private void GoBack(bool immediate = false)
        {
            _stopBackEase = immediate ? Time.time : Time.time + _backEaseDuration;
            _collider.isTrigger = true;
            _rigidbody.velocity = Vector2.zero;
            State = YoyoState.BACK;
        }

        private void Reset()
        {
            _quickShotCumDistance = 0;
            _collider.isTrigger = true;
            transform.position = _initPos.position;
            _rigidbody.velocity = Vector2.zero;
            State = YoyoState.IDLE;
            transform.SetParent(_parent);
            _owner.OnYoyoReturn();
        }

        private void DrawPath()
        {
            if (_currentLine == null) return;

            var position = transform.position;
            var d = Vector2.Distance(position, _currentLine.CurrentPosition);
            if (d > _resolution)
            {
                _currentLine.AddPosition(position);
            }
        }

        private void RemovePath(Line l)
        {
            if (l == null) return;

            Destroy(l.gameObject);
        }

        #endregion
    }
}