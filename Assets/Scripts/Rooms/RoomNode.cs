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

        #endregion

        #region Properties

        public bool Cleared
        {
            get => _cleared;
            set
            {
                _cleared = value;
                if (Room != null)
                    Room.GateState = GateState.OPEN;
            }
        }

        [field: SerializeField] public Vector2Int Index { get; private set; }
        [field: SerializeField] public Room Room { get; set; }
        [field: SerializeField] public int Rank { get; set; }
        [field: SerializeField] public int[] Enemies { get; private set; }

        #endregion

        #region Indexers

        public RoomNode this[Direction dir]
        {
            get => _nodes[(byte)dir];
            set => _nodes[(byte)dir] = value;
        }

        #endregion

        #region Constructors

        public RoomNode(Room room, Vector2Int index, int rank)
        {
            Room = room;
            Index = index;
            Rank = rank;
            Enemies = new int[RoomManager.EnemyDictionary.Count];
            ChooseEnemies();
            Cleared = Enemies.Sum() == 0;
        }

        #endregion

        #region Private Methods

        private void ChooseEnemies()
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
        }

        #endregion
    }
}