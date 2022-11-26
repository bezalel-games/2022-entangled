namespace Rooms.CardinalDirections
{
    public static class CardinalDirectionExtensions
    {
        private const int NumOfDirections = 4;

        public static CardinalDirection Inverse(this CardinalDirection dir)
        {
            return (CardinalDirection)((short)dir + 2 % NumOfDirections);
        }
    }
}