using System;
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

        public string ToString(string format)
        {
            return string.Format(format, _buff?.Name, _buff?.Description, _buff?.Rarity, 
                _debuff?.Name, _debuff?.Description, _debuff?.Rarity);
        }

        #endregion

        #region Private Methods

        #endregion
    }
}