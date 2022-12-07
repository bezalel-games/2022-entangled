using System;
using UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField] private GameObject _cards;
    [SerializeField] private Player.PlayerController _playerController;
    [SerializeField] private UIController _uiController;

    #endregion

    #region Non-Serialized Fields

    private static GameManager _instance;
    private CharacterMap _controls; // for input callbacks
    private bool _playerControllerEnabled;
    private bool _uiControllerEnabled;
    private ActionMap _actionMapInUse;

    #endregion

    #region Properties

    public static bool PlayerControllerEnabled
    {
        set
        {
            _instance._playerControllerEnabled = value;
            if (value && _instance._actionMapInUse is ActionMap.PLAYER)
                _instance._controls?.Player.Enable();
            else
                _instance._controls?.Player.Disable();
        }
    }

    public static bool UIControllerEnabled
    {
        set
        {
            _instance._uiControllerEnabled = value;
            if (value && _instance._actionMapInUse is ActionMap.UI)
                _instance._controls?.UI.Enable();
            else
                _instance._controls?.UI.Disable();
        }
    }

    private ActionMap ActionMapInUse
    {
        set
        {
            _actionMapInUse = value;
            switch (_actionMapInUse)
            {
                case ActionMap.PLAYER when _playerControllerEnabled:
                    _controls.Player.Enable();
                    _controls.UI.Disable();
                    break;
                case ActionMap.UI when _uiControllerEnabled:
                    _controls.UI.Enable();
                    _controls.Player.Disable();
                    break;
            }
        }
    }

    #endregion

    #region Function Events

    private void Awake()
    {
        if (_instance != null)
            throw new DoubleGameManagerException();
        _instance = this;
    }

    private void Start()
    {
        if (_controls == null)
        {
            _controls = new CharacterMap();
            _controls.Player.SetCallbacks(_playerController);
            _controls.UI.SetCallbacks(_uiController);
            ActionMapInUse = ActionMap.PLAYER;
        }
    }

    #endregion

    #region Public Methods

    public static void RoomCleared()
    {
        _instance.ChooseCard();
    }

    public static void CardChosen()
    {
        // OpenDoors();
        _instance._cards.SetActive(false);
        _instance.ActionMapInUse = ActionMap.PLAYER;
    }

    #endregion

    #region Private Methods

    private void ChooseCard()
    {
        ActionMapInUse = ActionMap.UI;
        _cards.SetActive(true);
    }

    #endregion

    #region Classes

    private class DoubleGameManagerException : Exception
    {
    }

    #endregion

    #region Enums

    private enum ActionMap : byte
    {
        PLAYER,
        UI,
    }

    #endregion
}