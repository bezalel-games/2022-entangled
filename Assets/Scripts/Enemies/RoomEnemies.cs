using Managers;
using Rooms;
using UnityEngine;

namespace Enemies
{
    public class RoomEnemies : MonoBehaviour
    {
        #region Non-Serialized Fields

        private int _numOfLivingEnemies;
        private int _enemiesTotal;

        #endregion

        #region Properties

        public RoomNode Node { get; set; }
        
        #endregion

        #region Public Methods

        public void Activate()
        {
            _enemiesTotal = transform.childCount;
            for (int i = 0; i < _enemiesTotal; ++i)
                transform.GetChild(i).gameObject.SetActive(true);
            _numOfLivingEnemies = _enemiesTotal;
        }

        public void RemoveEnemies()
        {
            var transformChildCount = transform.childCount;
            for (int i = 0; i < transformChildCount; ++i)
                Destroy(transform.GetChild(i).gameObject);
        }

        public void EnemyKilled()
        {
            GameManager.UpdateRoomCompletion(1 - (--_numOfLivingEnemies / (float)_enemiesTotal));
            if (_numOfLivingEnemies > 0) return;
            GameManager.RoomCleared();
        }

        #endregion

        #region Private Methods

        #endregion
    }
}