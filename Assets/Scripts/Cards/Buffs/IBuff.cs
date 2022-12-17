using Player;

namespace Cards.Buffs
{
    public interface IBuff : ICardProperty
    {
        public void Apply(PlayerController playerController);
    }
}