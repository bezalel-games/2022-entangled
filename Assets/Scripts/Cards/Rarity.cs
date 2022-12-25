using Unity.Collections;

namespace Cards
{
    public enum Rarity : byte
    {
        COMMON = 0,
        RARE = 1,
        EPIC = 2
    }

    public struct Rarities
    {
        private int _bits;

        public static Rarities All => new Rarities() { Bits = 0b0111 };

        public Rarities(Rarity rarity)
        {
            _bits = 1 << (int)rarity;
        }
        
        public int Bits
        {
            get => _bits;
            set => _bits = value & 0b0111;
        }

        // Return (true, <rarity>) if there is a single rarity - <rarity> otherwise return (false, -).
        public (bool onlyOneRarity, Rarity rarity) ToIntArrayNonAlloc(ref int[] rarities)
        {
            int soleRarity = -1;
            for (int shiftedBits = _bits, i = 0; i < 3; shiftedBits >>= 1, ++i)
            {
                if ((rarities[i] = shiftedBits & 1) > 0) // assignment and check
                    soleRarity = soleRarity == -1 ? i : -2;
            }

            return (soleRarity >= 0, (Rarity)soleRarity);
        }

        public static implicit operator Rarities(Rarity rarity) => new Rarities(rarity);
    }
}