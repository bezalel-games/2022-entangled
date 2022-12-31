using System;
using Cards.Buffs;
using Cards.Debuffs;
using Cards.Factory;
using Managers;
using Rooms;
using UnityEngine;

namespace Cards
{
    [Serializable]
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

        public BuffType BuffType => _buff.Type;
        public DebuffType DebuffType => _debuff.Type;

        public Sprite BuffSprite => _buff.CardSprite;
        public Sprite DebuffSprite => _debuff.CardSprite;

        public string BuffTitlePart => _buff.TitlePart;
        public string DebuffTitlePart => _debuff.TitlePart;

        public CardEssence Essence => new CardEssence(_buff, _debuff);

        #endregion

        #region Public Methods

        public void Apply(CardPool pool)
        {
            if (_applied) return;
            _buff.Apply(GameManager.PlayerController);
            _debuff.Apply(RoomManager.EnemyDictionary);
            _buff.UpdatePool(pool);
            _debuff.UpdatePool(pool);
            pool.FinishedUpdating();
            _applied = true;
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

        #region Classes

        [Serializable]
        public class CardEssence
        {
            public CardEssence(Buff buff, Debuff debuff)
            {
                Buff = (buff.Type, buff.Rarity);
                Debuff = (debuff.Type, debuff.Rarity);
            }

            public (BuffType type, Rarity rarity) Buff { get; }
            public (DebuffType type, Rarity rarity) Debuff { get; }
        }

        #endregion
    }
}