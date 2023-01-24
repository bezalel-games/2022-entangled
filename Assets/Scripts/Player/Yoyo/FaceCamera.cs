using UnityEngine;

namespace Player
{
    public class FaceCamera : MonoBehaviour
    {
        #region Function Events

        private void LateUpdate()
        {
            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);
        }

        #endregion
    }
}