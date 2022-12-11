using Player;

namespace Cards.Buffs
{
    public interface IBuff
    {
        public void Apply(PlayerController playerController);
    }
}