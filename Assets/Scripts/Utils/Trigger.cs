using System;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField] private UnityEvent _onTrigger;

    #endregion

    #region Non-Serialized Fields

    #endregion

    #region Properties

    #endregion

    #region Function Events

    private void OnTriggerEnter2D(Collider2D other)
    {
        _onTrigger.Invoke();
        gameObject.SetActive(false);
    }

    #endregion

    #region Public Methods

    #endregion

    #region Private Methods

    #endregion
}