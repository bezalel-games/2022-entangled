using System;
using Cards.CardElementClasses;
using Cards.Factory;
using Enemies;

namespace Cards.Debuffs
{
    public class SplitOnDeath : EnemyDebuff
    {
        private readonly int _splitCount;

        public SplitOnDeath(CardElementClassAttributes attributes, Rarity rarity, int enemyType, int splitCount)
            : base(attributes, rarity, enemyType)
        {
            _splitCount = splitCount;
        }

        public override DebuffType Type => DebuffType.SPLIT_GOOMBAS+_enemyType;

        public override void Apply(EnemyDictionary enemyDictionary)
        {
            EnemyDictionary.Entry entry = enemyDictionary[_enemyType];
            entry.SplitCount = _splitCount;
        }
    }
}