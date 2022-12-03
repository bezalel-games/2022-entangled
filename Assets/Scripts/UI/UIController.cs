using UnityEngine.InputSystem;

namespace UI
{
    public class UIController : MonoBehaviourExt, CharacterMap.IUIActions
    {
        #region Function Events

        private void OnEnable()
        {
            GameManager.UIControllerEnabled = true;
        }

        private void OnDisable()
        {
            GameManager.UIControllerEnabled = false;
        }

        #endregion

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

        #endregion
    }
}