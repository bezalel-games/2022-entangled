using System;
using System.Collections.Generic;
using Rooms.CardinalDirections;
using UnityEngine;
using Enemies;
using Direction = Rooms.CardinalDirections.Direction;
using Random = UnityEngine.Random;

namespace Rooms
{
    public class RoomManager : MonoBehaviour
    {
        #region Serialized Fields

        [Tooltip("The node of the room the character is currently in")] [SerializeField]
        private RoomNode _currentRoom;

        [Tooltip("The Room Component of the Room Prefab used for spawning all rooms")] [SerializeField]
        private Room _roomPrefab;

        [Tooltip("Time before the previous room is put to sleep after a room transition")] [SerializeField]
        private float _previousRoomSleepDelay = 1;

        [Tooltip("A scriptable object containing all the enemies to spawn in the game")] [SerializeField]
        private EnemyDictionary _enemyDictionary;

        [Tooltip("The global rank of all rooms. ~Temporary")] [SerializeField]
        private int _rank = 8;

        [Header("General Room properties")] [SerializeField]
        private int _roomWidth = 16;

        [SerializeField] private int _roomHeight = 9;
        [SerializeField] private int _roomVerticalIntersection = 1;
        [SerializeField] private int _roomHorizontalIntersection = 1;
        [SerializeField] private int _wallSize = 1;

        #endregion

        #region Non-Serialized Fields

        private static RoomManager _instance;
        private readonly List<Room> _roomPool = new();
        private int _enemySpawnLayerMask;

        #endregion

        #region Properties

        public static EnemyDictionary EnemyDictionary => _instance._enemyDictionary;

        #endregion

        #region Function Events

        private void Awake()
        {
            if (_instance != null)
                throw new DoubleRoomManagerException();
            _instance = this;
            _enemySpawnLayerMask = Physics2D.GetLayerCollisionMask(EnemyDictionary[0].gameObject.layer);
        }

        private void Start()
        {
            _currentRoom = new RoomNode(_currentRoom.Room, _currentRoom.Index, _currentRoom.Rank);
            _currentRoom.Room.Node = _currentRoom;
            LoadNeighbors(_currentRoom);
        }

        #endregion

        #region Public Method

        public static void ChangeRoom(RoomNode newRoom)
        {
            if (newRoom == _instance._currentRoom) return;
            var currPos = _instance._currentRoom.Index;
            var newPos = newRoom.Index;
            ChangeRoom(newRoom, (newPos - currPos).ToDirection());
        }

        public static void ChangeRoom(RoomNode newRoom, Direction dirOfNewRoom)
        {
            newRoom.Room.Enter();
            _instance._currentRoom.Room.Exit(_instance._previousRoomSleepDelay);
            _instance.UnloadNeighbors(_instance._currentRoom, dirOfNewRoom); // TODO: async?
            _instance.LoadNeighbors(newRoom, dirOfNewRoom.Inverse()); // TODO: async?
            _instance._currentRoom = newRoom;
        }

        #endregion

        #region Private Methods

        private void UnloadNeighbors(RoomNode prevRoom, Direction? dirOfNewRoom = null)
        {
            foreach (Direction dir in DirectionExt.GetDirections())
            {
                if (dir == dirOfNewRoom)
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
                var neighborNode = newRoomNode[dir];
                if (neighborNode == null)
                    // no neighbor node yet
                {
                    var index = newRoomNode.Index + dir.ToVector();
                    var room = GetRoom(index);
                    room.Node = new RoomNode(room, index, _rank);
                    room.Node[dir.Inverse()] = newRoomNode;
                    newRoomNode[dir] = room.Node;
                    SpawnEnemies(room.Node, room);
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
                SpawnEnemies(neighborNode, neighborRoom);
            }
        }

        private void SpawnEnemies(RoomNode roomNode, Room room)
        {
            var pos = GetPosition(roomNode.Index);
            for (int i = 0; i < roomNode.Enemies.Length; ++i)
                for (int j = roomNode.Enemies[i]; j > 0; j--)
                    Instantiate(EnemyDictionary[i], pos + ValidRandomPosInRoom(), Quaternion.identity, room.Enemies);
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
            room.ReplaceEnemies();
            room.Node.Room = null;
            room.Node = roomNode;
            room.transform.position = GetPosition(index);
            return room;
        }

        private Vector3 GetPosition(Vector2Int roomIndex)
        {
            return new Vector3(roomIndex.x * (_roomWidth - _roomVerticalIntersection),
                roomIndex.y * (_roomHeight - _roomHorizontalIntersection));
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
            var halfWidth = _roomWidth / 2 - _wallSize;
            var halfHeight = _roomHeight / 2 - _wallSize;
            var x = Random.Range(-halfWidth, halfWidth);
            var y = Random.Range(-halfHeight, halfHeight);
            return new Vector3(x, y);
        }

        private Vector3 ValidRandomPosInRoom()
        {
            var perimeter = new Vector3(0.5f, 0.5f);
            for (int i = 0; i < 10; i++)
            {
                var pos = RandomPosInRoom();
                if (Physics2D.OverlapArea(pos - perimeter, pos + perimeter, _enemySpawnLayerMask) == null)
                    return pos;
            }
            return Vector3.zero;
        }
        
        

        #endregion

        #region Classes

        private class DoubleRoomManagerException : Exception
        {
        }

        #endregion
    }
}