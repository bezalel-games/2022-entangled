using Cards.Buffs;
using Cards.Debuffs;
using Managers;
using Rooms;
using UnityEngine;

namespace Cards
{
    public class Card : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private IBuff _buff;
        [SerializeField] private IDebuff _debuff;

        #endregion

        #region Non-Serialized Fields

        private bool _applied = false;

        #endregion

        #region Properties

        #endregion

        #region Function Events

        #endregion

        #region Public Methods

        public void Apply()
        {
            if (_applied) return;
            _buff.Apply(GameManager.PlayerController);
            _debuff.Apply(RoomManager.EnemyDictionary);
            _applied = true;
        }

        #endregion

        #region Private Methods

        #endregion
    }
}