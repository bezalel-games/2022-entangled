using Cards.CardElementClasses;
using Cards.Cards;
using Cards.Factory;
using Player;

namespace Cards.Buffs
{
    public abstract class Buff : CardElement
    {
        public Buff(CardElementClassAttributes attributes, Rarity rarity) : base(attributes, rarity)
        {
        }

        public abstract void Apply(PlayerController playerController);
        
        public abstract BuffType Type { get; }
    }
}