using Enemies;

namespace Cards.Debuffs
{
    public interface IDebuff
    {
        public void Apply(EnemyDictionary enemyDictionary);
    }
}