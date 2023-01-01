using Cards.CardElementClasses;
using Cards.Factory;
using UnityEngine;

namespace Cards
{
    public abstract class CardElement
    {
        private readonly CardElementClassAttributes _attributes;
        public string Name => _attributes.Name;
        public string TitlePart => _attributes.TitlePart;
        public string Description => _attributes.Description;
        public Sprite CardSprite => _attributes.CardSprite;
        public Rarity Rarity { get; }

        protected CardElement(CardElementClassAttributes attributes, Rarity rarity)
        {
            _attributes = attributes;
            Rarity = rarity;
        }

        public void UpdatePool(CardPool pool)
        {
            var allRarities = Rarities.All;
            foreach (var buff in _attributes.UnlockedBuffs)
                pool.Add(buff, allRarities);
            foreach (var debuff in _attributes.UnlockedDebuffs)
                pool.Add(debuff, allRarities);
            if (_attributes.SingleUse)
                RemoveSelfFromPool(pool);
        }

        protected abstract void RemoveSelfFromPool(CardPool pool);
    }
}