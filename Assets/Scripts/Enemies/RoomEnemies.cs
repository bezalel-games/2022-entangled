using Managers;
using Rooms;
using UnityEngine;

namespace Enemies
{
    public class RoomEnemies : MonoBehaviour
    {
        #region Non-Serialized Fields

        private int _numOfLivingEnemies;

        #endregion

        #region Properties

        public RoomNode Node { get; set; }
        
        #endregion

        #region Public Methods

        public void Activate()
        {
            var transformChildCount = transform.childCount;
            for (int i = 0; i < transformChildCount; ++i)
                transform.GetChild(i).gameObject.SetActive(true);
            _numOfLivingEnemies = transformChildCount;
        }

        public void RemoveEnemies()
        {
            var transformChildCount = transform.childCount;
            for (int i = 0; i < transformChildCount; ++i)
                Destroy(transform.GetChild(i).gameObject);
        }

        public void EnemyKilled()
        {
            if (--_numOfLivingEnemies > 0) return;
            if (Node != null)
                Node.Cleared = true;
            GameManager.RoomCleared();
            
        }

        #endregion

        #region Private Methods

        #endregion
    }
}