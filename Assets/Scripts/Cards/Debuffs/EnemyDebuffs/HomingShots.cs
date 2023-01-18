using System;
using Cards.CardElementClasses;
using Cards.Factory;
using Enemies;

namespace Cards.Debuffs
{
    public class HomingShots : EnemyDebuff
    {
        public HomingShots(CardElementClassAttributes attributes, Rarity rarity, int enemyType)
            : base(attributes, rarity, enemyType) { }

        public override DebuffType Type => DebuffType.HOMING_SHOTS;

        public override void Apply(EnemyDictionary enemyDictionary)
        {
            EnemyDictionary.Entry entry = enemyDictionary[_enemyType];
            
            if (entry.Prefab is not Shooter)
                throw new HomingDebuffOnWrongEnemyException();

            entry.HomingShots = true;
        }

        private class HomingDebuffOnWrongEnemyException : Exception { }
    }
}