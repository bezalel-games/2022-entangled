using UnityEngine;

namespace Enemies
{
    public class EnemyChildScript : MonoBehaviour
    {
        #region Non-Serialized Fields

        private Enemy _parent;

        #endregion

        #region Function Events

        private void Awake()
        {
            _parent = GetComponentInParent<Enemy>();
        }

        #endregion

        #region Public Methods

        public void Die()
        {
            _parent.AfterDeathAnimation();
        }

        #endregion
    }
}