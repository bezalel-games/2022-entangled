using Cards.CardElementClasses;
using Enemies;

namespace Cards.Debuffs
{
    public abstract class EnemyDebuff : Debuff
    {
        protected readonly int _enemyType;

        protected EnemyDebuff(CardElementClassAttributes attributes, Rarity rarity, int enemyType) : base(attributes,
            rarity)
        {
            _enemyType = enemyType;
        }
        public abstract void Apply(EnemyDictionary enemyDictionary);
    }
}