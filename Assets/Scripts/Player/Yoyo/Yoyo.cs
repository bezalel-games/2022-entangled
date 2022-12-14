using Enemies;
using HP_System;
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
        [SerializeField] private float _backSpeed;

        [Header("Precision Shot")]
        [SerializeField] private float _precisionRotationSpeed;
        [SerializeField] private float _precisionSpeed;
        [SerializeField] private float _resolution;
        [SerializeField] private Line _linePrefab;
        [SerializeField] private float _timeScale;

        [Header("Other")]
        [SerializeField] private float _waitForReturn;

        [SerializeField] private Transform _parent;
        [SerializeField] private Transform _initPos;

        [SerializeField] private float _damage;

        #endregion

        #region Non-Serialized Fields
    
        private Rigidbody2D _rigidbody;
        private Collider2D _collider;

        private Vector3 _direction;
        private Vector2 _precisionDirection;

        private YoyoState _state = YoyoState.IDLE;
        private PlayerController _player;

        private Line _currentLine;

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
            _collider = GetComponent<Collider2D>();
        }

        protected override void Update()
        {
            base.Update();

            switch (_state)
            {
                case YoyoState.IDLE:
                    if(transform.position != _initPos.position)
                        transform.position = _initPos.position;
                    break;
                case YoyoState.SHOOT:
                    var d = Vector2.Distance(transform.position, _parent.position);
                    if (d > _maxDistance)
                    {
                        GoBack();
                    }
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
                    break;
                case YoyoState.PRECISION:
                    if (other.CompareTag("Wall"))
                    {
                        _collider.isTrigger = false;
                    }

                    if (other.CompareTag("Enemy"))
                    {
                        DoDamage(hittable);
                        if(enemy != null)
                            _player.OnHitEnemy(enemy);
                    }
                    break;
                case YoyoState.SHOOT:
                    if (other.CompareTag("Enemy"))
                    {
                        DoDamage(hittable);
                        if(enemy != null)
                            _player.OnHitEnemy(enemy);
                    }
                    GoBack();
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
                    if(other.CompareTag("Player"))
                        Reset();
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
                    _rigidbody.velocity = _direction.normalized * _shootSpeed;
                    break;
            
                case YoyoState.PRECISION:
                    _rigidbody.velocity = PrecisionDirection.normalized * _precisionSpeed * (1/_timeScale);
                    _player.OnPrecision();
                    break;
            
                case YoyoState.BACK:
                    var backDirection = ((Vector2)_parent.transform.position - (Vector2)transform.position);
                    _rigidbody.velocity = backDirection.normalized * _backSpeed;
                    break;
            }
        }

        private void DoDamage(IHittable hittable)
        {
            if(hittable == null) return;
            hittable.OnHit(_damage);
        }

        private void GoBack()
        {
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
            if(_currentLine == null) return;
        
            var position = transform.position;
            var d = Vector2.Distance(position, _currentLine.CurrentPosition);
            if (d > _resolution)
            {
                _currentLine.AddPosition(position);
            }
        }
    
        private void RemovePath()
        {
            if(_currentLine == null) return;
        
            Destroy(_currentLine.gameObject);
            _currentLine = null;
        }

        #endregion
    }
}