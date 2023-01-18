using System;
using System.Collections.Generic;
using Rooms;
using UnityEngine;
using Random = UnityEngine.Random;

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
            [SerializeField] private int _rank;

            #endregion

            #region Non-Serialized Fields

            private const float ENEMY_SPAWN_CLEAR_RADIUS = 2;
            private float _maxHp;
            private float _maxSpeed;
            private int _collisionLayerMask;

            #endregion

            #region Properties

            public int Rank
            {
                get => _rank;
                set
                {
                    _rank = Math.Max(1, value);
                    RoomManager.EnemyDictionary.OnValidate();
                }
            }

            public float MaxHp
            {
                get
                {
                    if (_maxHp == 0) _maxHp = Prefab.MaxHp;
                    return _maxHp;
                }
                set => _maxHp = value;
            }
            
            public float MaxSpeed
            {
                get
                {
                    if (_maxSpeed == 0) _maxSpeed = Prefab.MaxSpeed;
                    return _maxSpeed;
                }
                set => _maxSpeed = value;
            }

            public string Name => Prefab.gameObject.name;
            public bool HomingShots { get; set; }

            #endregion

            #region Public Methods

            public bool Spawn(Vector3 position, Transform parent, bool force = false)
            {
                if (_collisionLayerMask == 0)
                    _collisionLayerMask = Physics2D.GetLayerCollisionMask(Prefab.gameObject.layer);
                if (Physics2D.OverlapCircle(position, ENEMY_SPAWN_CLEAR_RADIUS, _collisionLayerMask) == null && !force)
                    return false;

                var spawnedEnemy = Instantiate(Prefab, position, Quaternion.identity, parent);
                spawnedEnemy.ToggleBarrier(Random.value >= RoomManager.GhostChance);

                // subscribe a method to update enemy type variables on it's enablement
                spawnedEnemy.Enabled += () =>
                {
                    spawnedEnemy.MaxHp = MaxHp;
                    spawnedEnemy.MaxSpeed = MaxSpeed;

                    if (spawnedEnemy is Shooter)
                        ((Shooter) spawnedEnemy).HomingShots = HomingShots;
                };

                return true;
            }

            #endregion
        }

        #endregion
    }
}