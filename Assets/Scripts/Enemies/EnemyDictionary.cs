using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    [CreateAssetMenu(fileName = "Enemy Dictionary Asset", menuName = "Enemies Asset", order = 0)]
    public class EnemyDictionary : ScriptableObject
    {
        [field: SerializeField] private Enemy[] Enemies { get; set; }

        public Enemy this[int index]
        {
            get => Enemies[index];
            set => Enemies[index] = value;
        }

        public int Count => Enemies.Length;

        private void OnValidate()
        {
            if (Enemies == null) return;
            Array.Sort(Enemies, (a, b) => Comparer<int>.Default.Compare(a.Rank, b.Rank));
        }

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
    }
}