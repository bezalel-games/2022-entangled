using Managers;
using UnityEngine;

namespace Enemies.Boss
{
    public class EndStairs : MonoBehaviour
    {
        private Collider2D _collider;
        private bool _open = false;

        #region Function Events

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player") && _open)
                GameManager.BossKilled();
        }

        #endregion

        #region Public Methods

        public void Open()
        {
            _collider.isTrigger = true;
            _open = true;
        }
    
        #endregion

        #region Private Methods

        #endregion
    }
}