using Cards.Buffs;
using Cards.Debuffs;
using Managers;
using Rooms;
using UnityEngine;

namespace Cards
{
    public class Card
    {
        #region Constructor

        public Card(IBuff buff, IDebuff debuff)
        {
            _buff = buff;
            _debuff = debuff;
        }
        
        #endregion
        
        #region Fields

        private readonly IBuff _buff;
        private readonly IDebuff _debuff;
        private bool _applied = false;

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        public void Apply()
        {
            if (_applied) return;
            _buff?.Apply(GameManager.PlayerController);
            _debuff?.Apply(RoomManager.EnemyDictionary);
            _applied = true;
        }

        #endregion

        #region Private Methods

        #endregion
    }
}