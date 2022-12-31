using Cards.CardElementClasses;
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

        public CardElement(CardElementClassAttributes attributes, Rarity rarity)
        {
            _attributes = attributes;
            Rarity = rarity;
        }
    }
}