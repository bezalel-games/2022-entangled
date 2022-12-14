using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    [CreateAssetMenu(fileName = "Enemy Dictionary Asset", menuName = "Entangled/Enemies Asset", order = 0)]
    public class EnemyDictionary : ScriptableObject
    {
        #region Serialized Fields

        [field: SerializeField] private Enemy[] Enemies { get; set; }

        #endregion

        #region Properties

        public int Count => Enemies.Length;

        #endregion

        #region Indexers

        public Enemy this[int index]
        {
            get => Enemies[index];
            private set => Enemies[index] = value;
        }

        #endregion

        #region Function Events

        private void OnValidate()
        {
            if (Enemies == null) return;
            Array.Sort(Enemies, (a, b) => Comparer<int>.Default.Compare(a.Rank, b.Rank));
        }

        #endregion

        #region Public Methods

        public int GetMaxIndexForRank(int rank)
        {
            for (int i = Enemies.Length - 1; i > 0; i--)
                if (Enemies[i].Rank <= rank)
                    return i;
            return 0;
        }

        public int GetRankByIndex(int index)
        {
            return Enemies[index].Rank;
        }

        #endregion
    }
}