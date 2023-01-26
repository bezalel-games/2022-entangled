using System;
using UnityEngine;

namespace Rooms
{
    public class StandaloneBossRoom : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private Room _bossRoom; 

        #endregion

        #region Non-Serialized Fields

        #endregion

        #region Events

        #endregion

        #region Properties

        #endregion

        #region Indexers

        #endregion

        #region Function Events

        private void Start()
        {
            _bossRoom.Node = new RoomNode(_bossRoom, Vector2Int.zero, 0, 1);
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion

    }
}