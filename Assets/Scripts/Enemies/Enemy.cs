using System;
using UnityEngine;

namespace Enemies
{
    public class Enemy : MonoBehaviour
    {
        #region Serialized Fields

        #endregion

        #region Non-Serialized Fields

        private RoomEnemies _roomEnemies;

        #endregion

        #region Properties

        [field: SerializeField] public int Rank { get; set; } = 1;

        #endregion

        #region Function Events

        private void Awake()
        {
            _roomEnemies = transform.parent.GetComponent<RoomEnemies>();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            _roomEnemies.EnemyKilled();
            gameObject.SetActive(false);
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion
    }
}