using System;
using System.Collections.Generic;
using Rooms.CardinalDirections;
using UnityEngine;
using Enemies;
using Managers;
using Player;
using Direction = Rooms.CardinalDirections.Direction;
using Random = UnityEngine.Random;

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

        [Tooltip("The global rank of all rooms. ~Temporary")] [SerializeField]
        private int _rank = 8;

        [SerializeField] private RoomProperties _roomProperties;
        [SerializeField] private bool _spawnEnemies = true;
        [SerializeField][Range(0f,1f)] private float _ghostChange = 0.66f;

        [Header("Maze settings")]
        [SerializeField] private int _minDistanceFromBoss = 5;
        [SerializeField] private int _maxDistanceFromBoss = 6;
        [SerializeField] private int _totalNumberOfRooms = 40;

        #endregion

        #region Non-Serialized Fields

        private RoomNode _currentRoom;
        private static RoomManager _instance;
        private readonly List<Room> _roomPool = new();
        private RoomNode _nextRoom;

        private INeighborsStrategy _strategy;

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
            GameManager.FinishedCurrentRoom += SpawnEnemiesInNeighbors;

            _strategy = new MazeStrategy(_minDistanceFromBoss, _maxDistanceFromBoss, _totalNumberOfRooms);
        }

        private void Start()
        {
            _enemyDictionary = Instantiate(_enemyDictionary); // duplicate to not overwrite the saved asset
            _currentRoom = new RoomNode(null, new Vector2Int(0, 0), _rank);
            _currentRoom.Room = GetRoom(_currentRoom.Index, _currentRoom);
            _currentRoom.Cleared = true;
            _currentRoom.Room.Enter();
            
            LoadNeighbors(_currentRoom);
            SpawnEnemiesInNeighbors();
            
            _currentRoom.Room.ShowOnMiniMap();
        }

        private void OnDestroy()
        {
            GameManager.FinishedCurrentRoom -= SpawnEnemiesInNeighbors;
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
            room.Enemies.RemoveEnemies();
            _instance.SpawnEnemies(room.Node);
        }

        public static void SpawnEnemiesInNeighbors()
        {
            foreach (Direction dir in DirectionExt.GetDirections())
            {
                var neighborNode = _instance._currentRoom[dir];
                if(neighborNode == null) continue;
                neighborNode.ChooseEnemies();
                _instance.SpawnEnemies(neighborNode);
            }
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
                SpawnEnemiesInNeighbors();
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
                if (dir == dirOfNewRoom || !_strategy.RoomExists(prevRoom.Index + dir.ToVector()))
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

                if (!_strategy.RoomExists(newRoomNode.Index + dir.ToVector()))
                {
                    newRoomNode[dir] = null;
                    continue;
                }

                var neighborNode = newRoomNode[dir];
                if (neighborNode == null)
                    // no neighbor node yet
                {
                    var index = newRoomNode.Index + dir.ToVector();
                    var room = GetRoom(index);
                    room.Node = new RoomNode(room, index, _rank);
                    room.Node[dir.Inverse()] = newRoomNode;
                    newRoomNode[dir] = room.Node;
                    continue;
                }

                if (neighborNode.Room != null && neighborNode.Room.Node == neighborNode)
                    // neighbor node exists and still has a room that is linked to it
                {
                    var index = _roomPool.FindIndex(room => room == neighborNode.Room);
                    RemoveAndReplaceFromPool(index);
                    continue;
                }

                // neighbor node exists but needs a new room
                var neighborRoom = GetRoom(neighborNode.Index, neighborNode);
                neighborNode.Room = neighborRoom;
                neighborNode[dir.Inverse()] = newRoomNode;
                newRoomNode[dir] = neighborNode;
            }
        }

        private void SpawnEnemies(RoomNode roomNode)
        {
            if (!_spawnEnemies || roomNode.Cleared) return;
            roomNode.Room.Enemies.RemoveEnemies();
            var roomCenter = GetPosition(roomNode.Index);
            var enemiesTransform = roomNode.Room.Enemies.transform;
            var numOfEnemyTypes = roomNode.Enemies.Length;
            for (int enemyType = 0; enemyType < numOfEnemyTypes; ++enemyType)
                for (int i = roomNode.Enemies[enemyType]; i > 0; i--)
                    SpawnEnemyInRandomPos(EnemyDictionary[enemyType], roomCenter, enemiesTransform);
        }

        private Room GetRoom(Vector2Int index, RoomNode roomNode = null)
        {
            Room room;
            if (_roomPool.Count == 0)
            {
                room = Instantiate(_roomPrefab, GetPosition(index), Quaternion.identity, transform);
                room.Node = roomNode;
                return room;
            }

            room = PopFromPool();
            room.Enemies.RemoveEnemies();
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
            float halfWidth = _roomProperties.Width / 2 - _roomProperties.WallSize;
            float halfHeight = _roomProperties.Height / 2 - _roomProperties.WallSize;
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

        #endregion

        #region Classes

        private class DoubleRoomManagerException : Exception
        {
        }

        #endregion
    }
}