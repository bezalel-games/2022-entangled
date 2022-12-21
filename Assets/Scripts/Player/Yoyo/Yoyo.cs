using Enemies;
using HP_System;
using Managers;
using Unity.VisualScripting;
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
        [SerializeField][Range(0f,1f)][Tooltip("Portion of \"Max Distance\" in which the weapon should slow down on quickshot")] 
        private float _easeDistance;
        [SerializeField][Tooltip("The lower this is, the slower the weapon will become before going back on quick shot")] 
        private float _easeVelocityThreshold = 1.5f;
        [SerializeField] private float _backSpeed;
        [SerializeField][Tooltip("The time it will take the weapon to reach \"Back Speed\" when going back")] 
        private float _backEaseDuration;
        [SerializeField][Tooltip("The higher this is, the more turning the right stick during quickshots will effect the direction of the weapon")] 
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

        [SerializeField] private float _damage;

        #endregion

        #region Non-Serialized Fields

        private Rigidbody2D _rigidbody;
        private Collider2D _collider;

        private Vector3 _direction;
        private Vector2 _precisionDirection;
        private Vector2 _quickShotLastPos;
        private Vector2 _quickShotDirection;
        private float _quickShotCumDistance;

        private YoyoState _state = YoyoState.IDLE;
        private PlayerController _player;

        private Line _currentLine;

        private float stopBackEase;

        #endregion

        #region Properties

        private Vector2 BackDirection =>
            ((Vector2) _parent.transform.position - (Vector2) transform.position).normalized;

        public YoyoState State => _state;

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

        #endregion

        #region Function Events

        private void Start()
        {
            _player = GetComponentInParent<PlayerController>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _collider = GetComponentInChildren<Collider2D>();
        }

        protected override void Update()
        {
            base.Update();

            switch (_state)
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

            if (_state != YoyoState.IDLE)
            {
                MoveYoyo();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var hittable = other.GetComponent<IHittable>();
            Enemy enemy = other.GetComponent<Enemy>();

            switch (_state)
            {
                case YoyoState.BACK:
                    if (other.CompareTag("Player"))
                    {
                        Reset();
                    }

                    if (other.CompareTag("Enemy"))
                    {
                        DoDamage(hittable);
                        if (enemy != null)
                            _player.OnHitEnemy(enemy);
                    }

                    break;
                case YoyoState.PRECISION:
                    if (other.CompareTag("Wall"))
                    {
                        _collider.isTrigger = false;
                    }

                    if (other.CompareTag("Enemy"))
                    {
                        DoDamage(hittable);
                        if (enemy != null)
                            _player.OnHitEnemy(enemy);
                    }

                    break;
                case YoyoState.SHOOT:
                    if(other.CompareTag("Player"))
                        return;
                    
                    if (other.CompareTag("Enemy"))
                    {
                        DoDamage(hittable);
                        if (enemy != null)
                            _player.OnHitEnemy(enemy);
                    }
                    
                    GoBack(true);
                    break;
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Wall"))
            {
                _collider.isTrigger = true;
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            switch (_state)
            {
                case YoyoState.BACK:
                    if (other.CompareTag("Player"))
                        Reset();
                    break;
                case YoyoState.SHOOT:
                    if (other.CompareTag("Enemy"))
                    {
                        var hittable = other.GetComponent<IHittable>();
                        Enemy enemy = other.GetComponent<Enemy>();
                        DoDamage(hittable);
                        if (enemy != null)
                            _player.OnHitEnemy(enemy);
                        
                        GoBack();
                    }

                    break;
            }
        }

        #endregion

        #region Public Methods

        public void Shoot(Vector3 direction)
        {
            if (direction != Vector3.zero)
            {
                _state = YoyoState.SHOOT;
                _direction = direction;
                _rigidbody.velocity = direction;
                _quickShotLastPos = transform.position;
                transform.SetParent(null);
            }
        }

        public void PrecisionShoot()
        {
            _state = YoyoState.PRECISION;

            if (_currentLine != null)
            {
                RemovePath();
            }

            GameManager.ScaleTime(_timeScale);

            transform.SetParent(null);
            _currentLine = Instantiate(_linePrefab, transform.position, Quaternion.identity);
        }

        public void CancelPrecision()
        {
            if (_currentLine == null) return;
            GameManager.ScaleTime(1);
            RemovePath();
            GoBack();
        }

        #endregion

        #region Private Methods

        private void MoveYoyo()
        {
            var vel = Vector2.zero;
            var perpendicular = Vector2.zero;
            var velWithPerpendicular = Vector2.zero;
            
            switch (_state)
            {
                case YoyoState.SHOOT:

                    var position = transform.position;

                    _quickShotCumDistance += Vector2.Distance(position, _quickShotLastPos);
                    _quickShotLastPos = position;
                    
                    vel = _rigidbody.velocity;
                    perpendicular = Vector2Ext.GetPerpendicularPortion(QuickShotDirection, vel);
                    velWithPerpendicular = (vel + _turnFactor * perpendicular).normalized; 
                    
                    if (_quickShotCumDistance < _easeDistance * _maxDistance)
                    {
                        _rigidbody.velocity = velWithPerpendicular * _shootSpeed;
                    }
                    else
                    {
                        /*
                         * for d = cumulativeDistance, e = easeDistance, m = maxDistance
                         * if d > em -> d = e*m + (m - e*m)*t -> t = (d - e*m)/(m - e*m)
                         */
                        var t = (_quickShotCumDistance - _easeDistance*_maxDistance) / (_maxDistance - _easeDistance*_maxDistance);
                        _rigidbody.velocity = Vector2.Lerp(velWithPerpendicular * vel.magnitude, Vector2.zero, t);
                    }

                    if(vel.magnitude <= _easeVelocityThreshold)
                        GoBack();
                    
                    break;

                case YoyoState.PRECISION:
                    _rigidbody.velocity = PrecisionDirection.normalized * _precisionSpeed * (1 / _timeScale);
                    _player.OnPrecision();
                    break;

                case YoyoState.BACK:
                    vel = BackDirection * _backSpeed;
                    perpendicular = Vector2Ext.GetPerpendicularPortion(QuickShotDirection, -vel);
                    velWithPerpendicular = (vel + _turnFactor * perpendicular).normalized; 
                    
                    if (Time.time > stopBackEase)
                    {
                        _rigidbody.velocity = velWithPerpendicular * _backSpeed;
                    }
                    else
                    {
                        var backT = 1 - ((stopBackEase - Time.time) / _backEaseDuration);
                        _rigidbody.velocity = Vector2.Lerp(_rigidbody.velocity, 
                             velWithPerpendicular * vel.magnitude, backT);
                    }
                    
                    // _rigidbody.velocity = (vel + _turnFactor*perpendicular).normalized * vel.magnitude;
                    
                    break;
            }
        }

        private void DoDamage(IHittable hittable)
        {
            if (hittable == null) return;
            hittable.OnHit(_damage);
        }

        private void GoBack(bool immediate=false)
        {
            stopBackEase = immediate ? Time.time : Time.time + _backEaseDuration;
            _collider.isTrigger = true;
            _rigidbody.velocity = Vector2.zero;
            _state = YoyoState.BACK;
        }

        private void Reset()
        {
            _quickShotCumDistance = 0;
            _collider.isTrigger = true;
            transform.position = _initPos.position;
            _rigidbody.velocity = Vector2.zero;
            _state = YoyoState.IDLE;
            transform.SetParent(_parent);
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

        private void RemovePath()
        {
            if (_currentLine == null) return;

            Destroy(_currentLine.gameObject);
            _currentLine = null;
        }

        #endregion
    }
}