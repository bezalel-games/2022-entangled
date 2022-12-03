using System;
using UnityEngine;

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
    [SerializeField] private float _precisionSpeed;
    [SerializeField] private float _resolution;
    [SerializeField] private Line _linePrefab;

    [Header("Other")]
    [SerializeField] private float _waitForReturn;

    [SerializeField] private Transform _parent;
    [SerializeField] private Transform _initPos;

    #endregion

    #region Non-Serialized Fields
    
    private Rigidbody2D _rigidbody;
    private Collider2D _collider;

    private Vector3 _direction;

    public YoyoState _state = YoyoState.IDLE;

    private Line _currentLine;

    #endregion

    #region Properties

    public YoyoState State => _state;
    
    public Vector2 PrecisionDirection { get; set; }

    #endregion

    #region Function Events

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }

    protected override void Update()
    {
        base.Update();

        switch (_state)
        {
            case YoyoState.IDLE:
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
                break;
            case YoyoState.SHOOT:
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
        
        transform.SetParent(null);
        _currentLine = Instantiate(_linePrefab, transform.position, Quaternion.identity);
    }

    public void CancelPrecision()
    {
        if (_currentLine == null) return; 
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
                _rigidbody.velocity = PrecisionDirection.normalized * _precisionSpeed;
                break;
            
            case YoyoState.BACK:
                var backDirection = ((Vector2)_parent.transform.position - (Vector2)transform.position);
                _rigidbody.velocity = backDirection.normalized * _backSpeed;
                break;
        }
    }

    private void GoBack()
    {
        _rigidbody.velocity = Vector2.zero;
        _state = YoyoState.BACK;
    }

    private void Reset()
    {
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