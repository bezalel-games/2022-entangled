using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField] private PlayerController _playerController;

    #endregion

    #region Non-Serialized Fields

    private static GameManager _instance;
    private CharacterMap _controls; // for input callbacks

    #endregion

    #region Properties

    #endregion

    #region Function Events

    private void Awake()
    {
        if (_instance != null)
            throw new DoubleGameManagerException();
        _instance = this;
        if (_controls == null)
        {
            // connect this class to callbacks from "Player" input actions
            _controls = new CharacterMap();
            _controls.Player.SetCallbacks(_playerController);
            _controls.Player.Enable();
        }
    }

    #endregion

    #region Public Methods

    public static void RoomCleared()
    {
        _instance.ChooseCard();
    }

    #endregion

    #region Private Methods

    private void ChooseCard()
    {
    }

    #endregion

    #region Classes

    private class DoubleGameManagerException : Exception
    {
    }

    #endregion
}