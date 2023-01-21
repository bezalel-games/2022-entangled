using System;
using Cards.CardElementClasses;
using Cards.Factory;
using Enemies;

namespace Cards.Debuffs
{
    public class StunningGoombas : EnemyDebuff
    {
        private float _stunDuration;

        public StunningGoombas(CardElementClassAttributes attributes, Rarity rarity, int enemyType, float duration)
            : base(attributes, rarity, enemyType)
        {
            _stunDuration = duration;
        }

        public override DebuffType Type => DebuffType.STUNNING_GOOMBAS;

        public override void Apply(EnemyDictionary enemyDictionary)
        {
            EnemyDictionary.Entry entry = enemyDictionary[_enemyType];
            
            if (entry.Prefab is not Goomba)
                throw new StunningGoombasWrongEnemyException();

            entry.StunningAttack = true;
            entry.StunDuration = _stunDuration;
        }

        private class StunningGoombasWrongEnemyException : Exception { }
    }
}