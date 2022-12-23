using System;
using UnityEngine;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private Canvas _slowdownFilter;
        [SerializeField] private Canvas _generalCanvas;

        #endregion

        #region Non-Serialized Fields

        private static UIManager _instance;

        #endregion

        #region Properties

        #endregion

        #region Function Events

        private void Awake()
        {
            if (_instance != null)
                throw new DoubleUIManagerException();
            _instance = this;
        }

        #endregion

        #region Public Methods

        public static void ToggleSlowdownFilter(bool show)
        {
            _instance._slowdownFilter.gameObject.SetActive(show);
        }

        public static void ToggleRunCanvas(bool show)
        {
            _instance._generalCanvas.gameObject.SetActive(show);
        }

        #endregion

        #region Private Methods

        #endregion

        #region Classes

        private class DoubleUIManagerException : Exception
        {
        }

        #endregion
    }
}