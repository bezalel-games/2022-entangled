using Cards.CardElementClasses;
using Cards.Cards;
using Cards.Factory;
using Enemies;

namespace Cards.Debuffs
{
    public abstract class Debuff : CardElement
    {
        public Debuff(CardElementClassAttributes attributes, Rarity rarity) : base(attributes, rarity)
        {
        }

        public abstract void Apply(EnemyDictionary enemyDictionary);

        public abstract DebuffType Type { get; }
    }
}