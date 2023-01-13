using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Rooms.CardinalDirections;
using Rooms.NeighborsStrategy;
using Direction = Rooms.CardinalDirections.Direction;
using Enemies;
using Interactables;
using Managers;
using Player;

namespace Rooms
{
    public class RoomManager : MonoBehaviour
    {
        #region Serialized Fields

        [Tooltip("The Room Component of the Room Prefab used for spawning all rooms")] [SerializeField]
        private Room _roomPrefab;

        [Tooltip("Time before the previous room is put to sleep after a room transition")] [SerializeField]
        private float _previousRoomSleepDelay = 1;

        [Tooltip("A scriptable object containing all the enemies to spawn in the game")] [SerializeField]
        private EnemyDictionary _enemyDictionary;

        [SerializeField]
        private int _minRoomRank = 20;

        [SerializeField] private RoomProperties _roomProperties;
        [SerializeField] private bool _spawnEnemies = true;
        [SerializeField] [Range(0f, 1f)] private float _ghostChange = 0.66f;
        [SerializeField] private InteractablePair[] _interactablePairs;

        [Header("Play mode")]
        [SerializeField] private NeighborsStrategy _playMode = NeighborsStrategy.MAZE;

        [Header("Maze settings")]
        [SerializeField] private int _minDistanceFromBoss = 5;

        [SerializeField] private int _maxDistanceFromBoss = 6;
        [SerializeField] private int _totalNumberOfRooms = 40;
        [SerializeField] private int _fountainCount = 1;
        [SerializeField] private int _treasureCount = 2;
        [SerializeField] private AnimationCurve _distanceToRankFunction;

        [Header("Boss room")]
        [SerializeField] private Room _bossRoomPrefab;

        #endregion

        #region Non-Serialized Fields

        private RoomNode _currentRoom;
        private static RoomManager _instance;
        private readonly List<Room> _roomPool = new();
        public static Dictionary<Vector2Int, RoomNode> Nodes { get; private set; } = new();
        private RoomNode _nextRoom;

        private INeighborsStrategy _strategy;
        public static Dictionary<RoomType, Interactable> Interactables { get; private set; } = new();

        #endregion

        #region Properties

        public static float GhostChance => _instance._ghostChange;

        public static EnemyDictionary EnemyDictionary => _instance._enemyDictionary;
        public static RoomProperties RoomProperties => _instance._roomProperties;

        private int ActualHalfWidth => _roomProperties.Width / 2 - _roomProperties.WallSize;
        private int ActualHalfHeight => _roomProperties.Height / 2 - _roomProperties.WallSize;

        #endregion

        #region Function Events

        private void Awake()
        {
            if (_instance != null)
                throw new DoubleRoomManagerException();
            _instance = this;
            GameManager.FinishedCurrentRoom += InitContentInNeighbors;
            GameManager.FinishedCurrentRoom += OpenCurrentRoomDoors;

            foreach (InteractablePair pair in _interactablePairs)
            {
                Interactables[pair.RoomType] = pair.Interactable;
            }
            
            InitStrategy();
        }

        private void Start()
        {
            _enemyDictionary = Instantiate(_enemyDictionary); // duplicate to not overwrite the saved asset
            var firstRoomIndex = Vector2Int.zero;
            _currentRoom = new RoomNode(null, firstRoomIndex, RoomRank(firstRoomIndex));
            _currentRoom.Room = GetRoom(_currentRoom.Index, _currentRoom);
            _currentRoom.Cleared = true;
            _currentRoom.Room.Enter();

            LoadNeighbors(_currentRoom);
            InitContentInNeighbors();

            _currentRoom.Room.ShowOnMiniMap();
        }

        private void OnDestroy()
        {
            GameManager.FinishedCurrentRoom -= InitContentInNeighbors;
            GameManager.FinishedCurrentRoom -= OpenCurrentRoomDoors;
        }

        #endregion

        #region Public Method

        public static void EnteredRoom(RoomNode roomNode)
        {
            _instance._nextRoom = roomNode;
        }

        public static void ExitedRoom(RoomNode roomNode, GameObject exitingObject)
        {
            if (_instance._nextRoom == null) return;
            if (_instance._currentRoom.Index == roomNode.Index)
                ChangeRoom(_instance._nextRoom, exitingObject);
            _instance._nextRoom = null;
        }

        public static void RepositionRoom(Room room)
        {
            room.transform.position = _instance.GetPosition(room.Node.Index);
            room.Clean();
            _instance.SpawnEnemies(room.Node);
        }

        #endregion

        #region Private Methods

        private static void ChangeRoom(RoomNode newRoom, GameObject transitioningObject)
        {
            if (newRoom == _instance._currentRoom) return;
            var indexDiff = newRoom.Index - _instance._currentRoom.Index;
            var dirOfNewRoom = indexDiff.ToDirection();
            var player = transitioningObject.GetComponent<PlayerController>();
            if (player)
            {
                MovePlayerToNewRoom(newRoom.Index, dirOfNewRoom, (Vector2)indexDiff, player);
            }
            
            newRoom.Room.Enter();
            _instance._currentRoom.Room.Exit(_instance._previousRoomSleepDelay);
            _instance.UnloadNeighbors(_instance._currentRoom, dirOfNewRoom); // TODO: async?
            _instance.LoadNeighbors(newRoom, dirOfNewRoom.Inverse()); // TODO: async?
            _instance._currentRoom = newRoom;
            _instance._currentRoom.Room.ShowOnMiniMap();
            if (newRoom.Cleared)
                InitContentInNeighbors();
        }

        private static void MovePlayerToNewRoom(Vector2Int newRoomIndex, Direction dirOfNewRoom, Vector3 walkDirection,
            PlayerController player)
        {
            var nextRoomPosition = _instance.GetPosition(newRoomIndex);
            float threshold = dirOfNewRoom switch
            {
                Direction.WEST => nextRoomPosition.x + _instance.ActualHalfWidth,
                Direction.EAST => nextRoomPosition.x - _instance.ActualHalfWidth,
                Direction.SOUTH => nextRoomPosition.y + _instance.ActualHalfHeight,
                Direction.NORTH => nextRoomPosition.y - _instance.ActualHalfHeight,
                _ => throw new ArgumentOutOfRangeException()
            };
            player.OverrideMovement(walkDirection.normalized, threshold);
        }

        private void UnloadNeighbors(RoomNode prevRoom, Direction? dirOfNewRoom = null)
        {
            foreach (Direction dir in DirectionExt.GetDirections())
            {
                if (dir == dirOfNewRoom 
                    || !_strategy.RoomExists(prevRoom.Index + dir.ToVector()))
                    continue;

                // Don't add room to pool if not existing or if boss room
                if(prevRoom[dir] == null || _strategy.IsBossRoom(prevRoom.Index + dir.ToVector()))
                    continue;
                
                var neighbor = prevRoom[dir].Room;
                _roomPool.Add(neighbor);
            }
        }

        private void LoadNeighbors(RoomNode newRoomNode, Direction? dirOfOldRoom = null)
        {
            foreach (Direction dir in DirectionExt.GetDirections())
            {
                if (dir == dirOfOldRoom)
                    continue;
                var roomIndex = newRoomNode.Index + dir.ToVector();

                if (!_strategy.RoomExists(roomIndex) || (_strategy.IsBossRoom(roomIndex) && dir != Direction.NORTH))
                {
                    newRoomNode[dir] = null;
                    continue;
                }

                var neighborNode = Nodes.ContainsKey(roomIndex) ? Nodes[roomIndex] : null;
                if (neighborNode == null)
                    // no neighbor node yet
                {
                    var isBossRoom = _strategy.IsBossRoom(roomIndex);
                    var room = GetRoom(roomIndex, isBossRoom: isBossRoom);
                    room.Node = new RoomNode(room, roomIndex, isBossRoom ? 0 : RoomRank(roomIndex));
                    room.Node[dir.Inverse()] = newRoomNode;
                    newRoomNode[dir] = room.Node;
                    continue;
                }

                if (neighborNode.Room != null && neighborNode.Room.Node == neighborNode)
                    // neighbor node exists and still has a room that is linked to it
                {
                    var poolIndex = _roomPool.FindIndex(room => room == neighborNode.Room);
                    RemoveAndReplaceFromPool(poolIndex);
                    continue;
                }
                
                // neighbor node exists but needs a new room
                var neighborRoom = GetRoom(neighborNode.Index, neighborNode);
                neighborNode.Room = neighborRoom;
                neighborNode[dir.Inverse()] = newRoomNode;
                newRoomNode[dir] = neighborNode;
            }
        }

        private int RoomRank(Vector2Int index) => _strategy.RoomRank(_minRoomRank, index, _distanceToRankFunction);

        private void SpawnEnemies(RoomNode roomNode)
        {
            if (!_spawnEnemies || roomNode.Cleared) return;
            roomNode.Room.Clean();
            var roomCenter = GetPosition(roomNode.Index);
            var enemiesTransform = roomNode.Room.Enemies.transform;
            var numOfEnemyTypes = roomNode.Enemies.Length;
            for (int enemyType = 0; enemyType < numOfEnemyTypes; ++enemyType)
                for (int i = roomNode.Enemies[enemyType]; i > 0; i--)
                    SpawnEnemyInRandomPos(EnemyDictionary[enemyType], roomCenter, enemiesTransform);
        }

        private Room GetRoom(Vector2Int index, RoomNode roomNode = null, bool isBossRoom = false)
        {
            Room room;
            if (isBossRoom)
            {
                room = Instantiate(_bossRoomPrefab, GetPosition(index), Quaternion.identity, transform);
                room.Node = roomNode;
                return room;
            }

            if (_roomPool.Count == 0)
            {
                room = Instantiate(_roomPrefab, GetPosition(index), Quaternion.identity, transform);
                room.Node = roomNode;
                return room;
            }

            room = PopFromPool();
            room.Clean();
            room.Node.Room = null;
            room.Node = roomNode;
            room.transform.position = GetPosition(index);
            return room;
        }

        private Vector3 GetPosition(Vector2Int roomIndex)
        {
            return new Vector3(roomIndex.x * (_roomProperties.Width - _roomProperties.VerticalIntersection),
                roomIndex.y * (_roomProperties.Height - _roomProperties.HorizontalIntersection));
        }

        private void RemoveAndReplaceFromPool(int index)
        {
            var last = PopFromPool();
            if (index == _roomPool.Count) return;
            _roomPool.RemoveAt(index);
            _roomPool.Insert(index, last);
        }

        private Room PopFromPool()
        {
            var lastIndex = _roomPool.Count - 1;
            var ret = _roomPool[lastIndex];
            _roomPool.RemoveAt(lastIndex);
            return ret;
        }

        private Vector3 RandomPosInRoom()
        {
            float halfWidth = _roomProperties.Width / 2 - _roomProperties.WallSize - _roomProperties.EnemySpawnMargin;
            float halfHeight = _roomProperties.Height / 2 - _roomProperties.WallSize - _roomProperties.EnemySpawnMargin;
            var x = Random.Range(-halfWidth, halfWidth);
            var y = Random.Range(-halfHeight, halfHeight);
            return new Vector3(x, y);
        }

        private void SpawnEnemyInRandomPos(EnemyDictionary.Entry enemyEntry, Vector3 roomCenter,
            Transform enemiesTransform)
        {
            for (int i = 0; i < 20; i++)
            {
                if (enemyEntry.Spawn(roomCenter + RandomPosInRoom(), enemiesTransform))
                    return;
            }

            enemyEntry.Spawn(roomCenter + RandomPosInRoom(), enemiesTransform, force: true);
        }

        private void InitStrategy()
        {
            _strategy = _playMode switch
            {
                NeighborsStrategy.MAZE => new MazeStrategy(_minDistanceFromBoss, _maxDistanceFromBoss,
                    _totalNumberOfRooms, _fountainCount, _treasureCount),
                NeighborsStrategy.ENDLESS => new EndlessStrategy(),
                NeighborsStrategy.BOSS => new BossStrategy(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }


        private static void InitContentInNeighbors()
        {
            _instance.InitContentInNeighbors_Inner();
        }

        private void InitContentInNeighbors_Inner()
        {
            foreach (Direction dir in DirectionExt.GetDirections())
            {
                var neighborNode = _currentRoom[dir];
                if (neighborNode == null) continue;

                RoomType type = _strategy.RoomType(neighborNode.Index);
                switch (type)
                {
                    case RoomType.MONSTERS:
                        if (!neighborNode.Cleared)
                        {
                            neighborNode.ChooseEnemies();
                            SpawnEnemies(neighborNode);
                        }
                        break;
                    case RoomType.FOUNTAIN:
                    case RoomType.TREASURE:
                        neighborNode.Room.InitInteractable(type);
                        neighborNode.Cleared = true;
                        break;
                }
            }
        }

        private void OpenCurrentRoomDoors()
        {
            _currentRoom.Cleared = true;
        }

        #endregion

        #region Types

        private class DoubleRoomManagerException : Exception
        {
        }

        private enum NeighborsStrategy : byte
        {
            MAZE,
            ENDLESS,
            BOSS,
        }
        
        [Serializable]
        public struct InteractablePair
        {
            public RoomType RoomType;
            public Interactable Interactable;
        }

        #endregion
    }
}