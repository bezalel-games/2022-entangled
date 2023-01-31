using System;
using System.Linq;
using Rooms.CardinalDirections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Rooms
{
    [Serializable]
    public class RoomNode
    {
        #region Non-Serialized Fields

        [NonSerialized] private RoomNode[] _nodes = new RoomNode[4];
        private bool _cleared = false;
        private bool _interacted = false;

        #endregion

        #region Properties

        public bool Cleared
        {
            get => _cleared;
            set
            {
                _cleared = value;
                // if (Room != null)
                //     Room.GateClosing = !value;
                if(_cleared && _interacted)
                    MinimapManager.SetCleared(Index);
            }
        }

        public bool Interacted
        {
            get => _interacted;
            set
            {
                _interacted = value;
                if(_cleared && _interacted)
                    MinimapManager.SetCleared(Index);
            }
        }

        [field: SerializeField] public Vector2Int Index { get; private set; }
        [field: SerializeField] public Room Room { get; set; }
        [field: SerializeField] public int Rank { get; set; }
        [field: SerializeField] public int[] Enemies { get; private set; }
        public float Intensity { get; }

        #endregion

        #region Indexers

        public RoomNode this[Direction dir]
        {
            get => _nodes[(byte) dir];
            set
            {
                _nodes[(byte) dir] = value;
                if(Room != null)
                    Room.ShowDoor(dir, value != null);
            }
        }

        #endregion

        #region Constructors

        public RoomNode(Room room, Vector2Int index, int rank, float intensity)
        {
            Room = room;
            Index = index;
            Rank = rank;
            Intensity = intensity;
            Enemies = new int[RoomManager.EnemyDictionary.Count];
            if (Room == null) return;
            foreach (Direction dir in DirectionExt.GetDirections())
                Room.ShowDoor(dir, false);
        }

        #endregion

        #region Public Methods

        public void ChooseEnemies()
        {
            var enemyDict = RoomManager.EnemyDictionary;
            var rankDelta = Rank;
            while (rankDelta > 0)
            {
                var maxIndex = enemyDict.GetMaxIndexForRank(rankDelta);
                if (maxIndex == -1) return;
                var chosenInd = Random.Range(0, maxIndex + 1);
                Enemies[chosenInd]++;
                rankDelta -= enemyDict.GetRankByIndex(chosenInd);
            }

            Cleared = Enemies.Sum() == 0;
        }

        #endregion
    }
}