using System;
using UnityEngine;

namespace Enemies
{
    public class RoomEnemies : MonoBehaviour
    {
        #region Serialized Fields

        #endregion

        #region Non-Serialized Fields

        private int _numOfLivingEnemies;

        #endregion

        #region Events

        public event Action RoomCleared;

        #endregion

        #region Properties

        #endregion

        #region Function Events

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
            RoomCleared?.Invoke();
        }

        #endregion

        #region Private Methods

        #endregion
    }
}