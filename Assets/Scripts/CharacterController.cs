using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour, CharacterMap.IPlayerActions
{
    # region Fields

    [Range(1,10)]
    [SerializeField] private float _speed = 2;
    
    private Rigidbody2D _rigidbody;
    private Vector2 _direction;
    
    private CharacterMap controls; // for input callbacks
    
    # endregion

    #region Properties

    private Vector2 Velocity => _direction * _speed;

    #endregion
    
    # region MonoBehaviour
    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    
    private void OnEnable()
    {
        if (controls == null)
        {
            // connect this class to callbacks from "Player" input actions
            controls = new CharacterMap();
            controls.Player.SetCallbacks(this);
        }
        
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    #endregion

    #region ActionMap

    public void OnMove(InputAction.CallbackContext context)
    {
        var inputDirection = Vector2.zero;
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                inputDirection = context.ReadValue<Vector2>();
                _direction = inputDirection;
                _rigidbody.velocity = Velocity;
                break;
            case InputActionPhase.Canceled:
                _direction = Vector2.zero;
                _rigidbody.velocity = Velocity;
                break;
        }
    }

    #endregion
}
