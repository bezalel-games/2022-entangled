using System;
using Cards.Buffs;
using Cards.Debuffs;
using Managers;
using Rooms;

namespace Cards
{
    public class Card
    {
        #region Constructor

        public Card(Buff buff, Debuff debuff)
        {
            _buff = buff;
            _debuff = debuff;
        }

        #endregion

        #region Fields

        private readonly Buff _buff;
        private readonly Debuff _debuff;
        private bool _applied = false;

        #endregion

        #region Properties

        public Rarity Rarity =>
            _buff.Rarity <= _debuff.Rarity ? Rarity.COMMON : (Rarity)(_buff.Rarity - _debuff.Rarity);

        #endregion

        #region Public Methods

        public void Apply()
        {
            if (_applied) return;
            _buff?.Apply(GameManager.PlayerController);
            _debuff?.Apply(RoomManager.EnemyDictionary);
            _applied = true;
        }

        public string TitleString(string format)
        {
            throw new NotImplementedException();
        }

        public string BuffString(string format)
        {
            return string.Format(format, _buff?.Name, _buff?.Description, _buff?.Rarity);
        }

        public string DebuffString(string format)
        {
            return string.Format(format, _debuff?.Name, _debuff?.Description, _debuff?.Rarity);
        }

        #endregion

        #region Private Methods

        #endregion
    }
}