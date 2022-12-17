using Enemies;

namespace Cards.Debuffs
{
    public interface IDebuff : ICardProperty
    {
        public void Apply(EnemyDictionary enemyDictionary);
    }
}