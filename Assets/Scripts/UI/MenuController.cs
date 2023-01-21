using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class MenuController : MonoBehaviour, CharacterMap.IMenuActions
    {
        [SerializeField] private GameObject _bossButton;

        #region Action Map
        
        public void OnNavigate(InputAction.CallbackContext context)
        {
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
        }

        public void OnClick(InputAction.CallbackContext context)
        {
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
        }

        public void OnMiddleClick(InputAction.CallbackContext context)
        {
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
        }

        public void OnTrackedDevicePosition(InputAction.CallbackContext context)
        {
        }

        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
        {
        }

        public void OnBossOption(InputAction.CallbackContext context)
        {
            if (context.started)
                _bossButton.SetActive(!_bossButton.activeSelf);
        }
        
        #endregion
    }
}