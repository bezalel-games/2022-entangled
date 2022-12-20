using Cards.CardElementClasses;

namespace Cards
{
    namespace Cards
    {
        public abstract class CardElement
        {
            private readonly CardElementClassAttributes _attributes;
            public string Name => _attributes.Name;
            public string Description => _attributes.Description;
            public Rarity Rarity { get; }

            public CardElement(CardElementClassAttributes attributes, Rarity rarity)
            {
                _attributes = attributes;
                Rarity = rarity;
            }
        }
    }
}