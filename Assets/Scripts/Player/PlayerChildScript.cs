using UnityEngine;

namespace Player
{
    public class PlayerChildScript : MonoBehaviour
    {
        #region Serialized Fields
  
        #endregion
        #region Non-Serialized Fields

        private PlayerController _parent;

        #endregion

        #region Properties

        #endregion

        #region Function Events

        private void Awake()
        {
            _parent = GetComponentInParent<PlayerController>();
        }

        #endregion

        #region Public Methods

        public void Die()
        {
            _parent.AfterDeathAnimation();
        }

        #endregion

        #region Private Methods

        #endregion
    }
}