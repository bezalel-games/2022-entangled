using System;
using Rooms.CardinalDirections;

namespace Rooms
{
    [Serializable]
    public class RoomNode
    {
        public Room Room { get; private set; }

        private readonly RoomNode[] _neighbors = new RoomNode[4];

        public RoomNode this[CardinalDirection dir]
        {
            get => _neighbors[(short)dir];
            set => _neighbors[(short)dir] = value;
        }

        public RoomNode(Room room)
        {
            Room = room;
        }
    }
}