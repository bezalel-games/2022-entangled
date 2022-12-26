using System;
using HP_System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
  #region Serialized Fields
  
  #endregion

  #region Non-Serialized Fields

  private Rigidbody2D _rigidbody;
  private Vector2 _direction;
  private float _speed;
  
  #endregion

  #region Properties

  public Vector2 Direction
  {
    get => _direction;
    set
    {
      _direction = value.normalized;
      var angle = Vector2.SignedAngle(Vector2.up, _direction);
      transform.rotation = Quaternion.Euler(0,0,angle);
    }
  }

  public float Speed
  {
    get => _speed;
    set => _speed = Mathf.Max(value, 0);
  }

  public float Damage { get; set; }
  
  public Action OnDisappear { get; set; }

  #endregion

  #region Function Events

  private void Awake()
  {
    _rigidbody = GetComponent<Rigidbody2D>();
  }

  private void FixedUpdate()
  {
    _rigidbody.velocity = Direction * _speed;
  }

  private void OnCollisionEnter2D(Collision2D other)
  {
    if (other.gameObject.CompareTag("Player"))
    {
      var hittable = other.gameObject.GetComponent<IHittable>();
      hittable.OnHit(transform, Damage);
    }
    
    OnDisappear?.Invoke();
  }

  private void OnBecameInvisible()
  {
    OnDisappear?.Invoke();
  }

  #endregion

  #region Public Methods

  #endregion

  #region Private Methods

  #endregion
}

