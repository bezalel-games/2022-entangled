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
        private Entry[] _sortedEnemies;

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
            for (int i = 0; i < Enemies.Length; i++)
            {
                Enemies[i].IndexInDictionary = i;
            }
            _sortedEnemies = new Entry[Enemies.Length];
            Array.Copy(Enemies, _sortedEnemies, _sortedEnemies.Length);
            Array.Sort(_sortedEnemies, (a, b) => Comparer<int>.Default.Compare(a.Rank, b.Rank));
        }
        
        private void OnEnable()
        {
            if (Enemies == null) return;
            for (int i = 0; i < Enemies.Length; i++)
            {
                Enemies[i].IndexInDictionary = i;
            }
            _sortedEnemies = new Entry[Enemies.Length];
            Array.Copy(Enemies, _sortedEnemies, _sortedEnemies.Length);
            Array.Sort(_sortedEnemies, (a, b) => Comparer<int>.Default.Compare(a.Rank, b.Rank));
        }

        #endregion

        #region Public Methods

        public int GetMaxIndexForRank(int rank)
        {
            for (int i = _sortedEnemies.Length - 1; i >= 0; i--)
                if (_sortedEnemies[i].Rank <= rank)
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
            
            public int IndexInDictionary;

            // More Enemies Debuff
            public int Rank
            {
                get => _rank;
                set
                {
                    _rank = Math.Max(1, value);
                    RoomManager.EnemyDictionary.OnValidate();
                }
            }

            // Stronger Enemies Debuff
            public float MaxHp
            {
                get
                {
                    if (_maxHp == 0) _maxHp = Prefab.MaxHp;
                    return _maxHp;
                }
                set => _maxHp = value;
            }
            
            // Faster Enemies Debuff
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
            
            // Homing Shot Debuff
            public bool HomingShots { get; set; }
            
            // Splitting Enemy Debuff
            public int SplitCount { get; set; }
            
            // Stunning Goombas Debuff
            public bool StunningAttack { get; set; }
            public float StunDuration { get; set; }

            #endregion

            #region Public Methods

            public (bool success, Enemy enemy) Spawn(Vector3 position, Transform parent, bool force = false, float ghostChance = 0)
            {
                if (_collisionLayerMask == 0)
                    _collisionLayerMask = Physics2D.GetLayerCollisionMask(Prefab.gameObject.layer);
                if (Physics2D.OverlapCircle(position, ENEMY_SPAWN_CLEAR_RADIUS, _collisionLayerMask) == null && !force)
                    return (false, null);

                var spawnedEnemy = Instantiate(Prefab, position, Quaternion.identity, parent);
                spawnedEnemy.Barrier.Active = Random.value < ghostChance;

                // subscribe a method to update enemy type variables on it's enablement
                spawnedEnemy.Enabled += () =>
                {
                    spawnedEnemy.Index = IndexInDictionary;
                    spawnedEnemy.MaxHp = MaxHp;
                    spawnedEnemy.MaxSpeed = MaxSpeed;
                    spawnedEnemy.SplitCount = SplitCount;

                    switch (spawnedEnemy)
                    {
                        case Shooter shooter:
                            shooter.HomingShots = HomingShots;
                            break;
                        case Goomba goomba:
                            goomba.StunningAttack = StunningAttack;
                            goomba.StunDuration = StunDuration;
                            break;
                    }
                };

                return (true, spawnedEnemy);
            }

            #endregion
        }

        #endregion
    }
}