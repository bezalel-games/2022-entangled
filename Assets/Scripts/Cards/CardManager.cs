using System;
using Cards.Buffs.ActiveBuffs;
using Cards.Buffs.PassiveBuffs;

namespace Cards
{
    public class CardManager
    {
        private Card _leftCard = new Card(new EnlargeYoyo(1.3f), null);
        private Card _rightCard = new Card(new SwapPositionWithYoyo(), null);

        public Card this[Side side] => side switch
        {
            Side.LEFT => _leftCard,
            Side.RIGHT => _rightCard,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
        };
    }
[Serializable]
    public enum Side : byte
    {
        LEFT,
        RIGHT
    }
}