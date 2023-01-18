using Cards.CardElementClasses;
using Enemies;

namespace Cards.Debuffs
{
    public abstract class EnemyDebuff : Debuff
    {
        protected EnemyDebuff(CardElementClassAttributes attributes, Rarity rarity) : base(attributes, rarity) {}
        public abstract void Apply(EnemyDictionary enemyDictionary);
    }
}