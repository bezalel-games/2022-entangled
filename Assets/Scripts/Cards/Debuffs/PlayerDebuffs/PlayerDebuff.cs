using Cards.CardElementClasses;
using Player;

namespace Cards.Debuffs
{
    public abstract class PlayerDebuff : Debuff
    {
        protected PlayerDebuff(CardElementClassAttributes attributes, Rarity rarity) : base(attributes, rarity) {}

        public abstract void Apply(PlayerController player);
    }
}