using System;
using UnityEngine;

public class Yoyo : MonoBehaviour
{
    public enum YoyoState
    {
        IDLE,
        SHOOT,
        BACK,
        PERCISION
    }

    #region Serialized Fields

    [SerializeField] private float _shootSpeed;
    [SerializeField] private float _backSpeed;
    [SerializeField] private float _waitForReturn;
    [SerializeField] private float _maxDistance;

    [SerializeField] private Transform _parent;
    [SerializeField] private Transform _initPos;

    #endregion

    #region Non-Serialized Fields
    
    private Rigidbody2D _rigidbody;

    private Vector3 _direction;
    public YoyoState _state = YoyoState.IDLE;

    #endregion

    #region Properties

    public YoyoState State => _state;

    #endregion

    #region Function Events

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (_state == YoyoState.IDLE)
        {
            transform.position = _initPos.position;
        }
        var d = Vector2.Distance(transform.position, _parent.position);
        if (_state == YoyoState.SHOOT && d > _maxDistance)
        {
            GoBack();
        }
    }

    private void FixedUpdate()
    {
        if (_state != YoyoState.IDLE)
        {
            MoveYoyo();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && _state == YoyoState.BACK)
        {
            Reset();
            return;
        }

        if (_state == YoyoState.SHOOT)
        {
            GoBack();
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

    #endregion

    #region Private Methods

    private void MoveYoyo()
    {
        switch (_state)
        {
            case YoyoState.SHOOT:
                _rigidbody.velocity = _direction.normalized * _shootSpeed;
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

    #endregion
}