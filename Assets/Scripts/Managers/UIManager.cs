using System;
using UnityEngine;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private Canvas _slowdownFilter;
        [SerializeField] private Canvas _generalCanvas;
        [SerializeField] private Canvas _pauseMenu;

        #endregion

        #region Non-Serialized Fields

        private static UIManager _instance;

        #endregion

        #region Properties

        public static bool Paused => _instance._pauseMenu.gameObject.activeSelf;

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

        public static void TogglePauseMenu(bool show)
        {
            _instance._pauseMenu.gameObject.SetActive(show);
        }
    }
}