using Enemies;
using HP_System;
using Managers;
using Unity.VisualScripting;
using UnityEngine;

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
        [SerializeField] private float _easeDistance;
        [SerializeField] private float _easeVelocityThreshold = 1.5f;
        [SerializeField] private float _backSpeed;
        [SerializeField] private float _backEaseDuration;

        [Header("Precision Shot")] [SerializeField]
        private float _precisionRotationSpeed;

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
        private Vector2 _quickShotPosition;

        private YoyoState _state = YoyoState.IDLE;
        private PlayerController _player;

        private Line _currentLine;

        private float stopBackEase;

        #endregion

        #region Properties

        public YoyoState State => _state;

        public Vector2 PrecisionDirection
        {
            get => _precisionDirection;
            set => _precisionDirection =
                Vector2.Lerp(_precisionDirection, value, _precisionRotationSpeed * Time.unscaledTime);
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
                _quickShotPosition = _initPos.position + (_maxDistance*_direction.normalized);
                transform.SetParent(null);

                //HitCloseByEnemies();
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

            // DelayInvoke(CancelPrecision, _precisionTime*_timeScale);
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
            switch (_state)
            {
                case YoyoState.SHOOT:
                    var distance = ((Vector2) transform.position - _quickShotPosition).magnitude;

                    var vel = _rigidbody.velocity;
                    if (distance > _easeDistance * _maxDistance)
                    {
                        vel = _direction.normalized * _shootSpeed;
                    }
                    else
                    {
                        var t = 1 - (distance / (_easeDistance * _maxDistance));
                        vel = Vector2.Lerp(_rigidbody.velocity, Vector2.zero, t);
                    }

                    _rigidbody.velocity = vel;
                    
                    if(vel.magnitude <= _easeVelocityThreshold)
                        GoBack();
                    
                    break;

                case YoyoState.PRECISION:
                    _rigidbody.velocity = PrecisionDirection.normalized * _precisionSpeed * (1 / _timeScale);
                    _player.OnPrecision();
                    break;

                case YoyoState.BACK:
                    var backDirection = ((Vector2) _parent.transform.position - (Vector2) transform.position);
                    var backT = 1 - ((stopBackEase - Time.time) / _backEaseDuration);
                    if (backT < 0)
                    {
                        _rigidbody.velocity = backDirection.normalized * _backSpeed;
                    }
                    else
                    {
                        _rigidbody.velocity = Vector2.Lerp(_rigidbody.velocity, backDirection.normalized * _backSpeed, backT);
                    }
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