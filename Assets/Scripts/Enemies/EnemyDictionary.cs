using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    [CreateAssetMenu(fileName = "Enemy Dictionary Asset", menuName = "Entangled/Enemies Asset", order = 0)]
    public class EnemyDictionary : ScriptableObject
    {
        #region Serialized Fields

        [field: SerializeField] private Entry[] Enemies { get; set; }

        #endregion

        #region Properties

        public int Count => Enemies.Length;

        #endregion

        #region Indexers

        public Entry this[int index]
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
            for (int i = Enemies.Length - 1; i >= 0; i--)
                if (Enemies[i].Rank <= rank)
                    return i;
            return -1;
        }

        public int GetRankByIndex(int index)
        {
            return Enemies[index].Rank;
        }

        #endregion

        #region Nested Classes

        [Serializable]
        public class Entry
        {
            #region Serialized Fields

            [field: SerializeField] public Enemy Prefab { get; set; }
            [field: SerializeField] public int Rank { get; set; }

            #endregion

            #region Non-Serialized Fields

            private const float ENEMY_SPAWN_CLEAR_RADIUS = 2;
            private float _maxHp;
            private int _collisionLayerMask;

            #endregion

            #region Properties

            public float MaxHp
            {
                get
                {
                    if (_maxHp == 0) _maxHp = Prefab.MaxHp;
                    return _maxHp;
                }
                set
                {
                    _maxHp = value;
                    MaxHpUpdate?.Invoke(value);
                }
            }

            public string Name => Prefab.gameObject.name;

            #endregion

            #region Events

            private event Action<float> MaxHpUpdate;

            #endregion

            #region Public Methods

            public bool Spawn(Vector3 position, Transform parent, bool force = false)
            {
                if (_collisionLayerMask == 0)
                    _collisionLayerMask = Physics2D.GetLayerCollisionMask(Prefab.gameObject.layer);
                if (force)
                    position = Vector3.zero;
                else if (Physics2D.OverlapCircle(position, ENEMY_SPAWN_CLEAR_RADIUS, _collisionLayerMask) == null)
                    return false;

                var spawnedEnemy = Instantiate(Prefab, position, Quaternion.identity, parent);

                // subscribe a method to update enemy HP on it's enablement
                spawnedEnemy.Enabled += () => spawnedEnemy.MaxHp = MaxHp;
                
                return true;
            }

            #endregion
        }

        #endregion
    }
}