using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Serialized Fields

    private static GameManager _instance;

    #endregion

    #region Non-Serialized Fields

    #endregion

    #region Properties

    #endregion

    #region Function Events

    private void Awake()
    {
        if (_instance != null)
            throw new DoubleGameManagerException();
        _instance = this;
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