using System;
using System.Collections.Generic;
using Rooms.CardinalDirections;
using UnityEngine;

namespace Rooms
{
    public class RoomManager : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private RoomNode currentRoom;
        [SerializeField] private GameObject roomPrefab;
        [SerializeField] private int roomWidth = 16;
        [SerializeField] private int roomHeight = 9;
        [SerializeField] private int roomVerticalIntersection = 1;
        [SerializeField] private int roomHorizontalIntersection = 1;
        [SerializeField] private float previousRoomSleepDelay = 1;

        #endregion

        #region Non-Serialized Fields

        private static RoomManager _instance;
        private readonly List<Room> _roomPool = new();

        #endregion

        #region Properties

        #endregion

        #region Function Events

        private void Awake()
        {
            if (_instance != null)
                throw new DoubleRoomManagerException();
            _instance = this;
        }

        private void Start()
        {
            currentRoom = new RoomNode(currentRoom.Room, currentRoom.Index);
            currentRoom.Room.Node = currentRoom;
            LoadNeighbors(currentRoom);
        }

        #endregion

        #region Public Method

        public static void ChangeRoom(RoomNode newRoom)
        {
            if (newRoom == _instance.currentRoom) return;
            var currPos = _instance.currentRoom.Index;
            var newPos = newRoom.Index;
            ChangeRoom(newRoom, (newPos - currPos).ToDirection());
        }

        public static void ChangeRoom(RoomNode newRoom, Direction dirOfNewRoom)
        {
            newRoom.Room.Enter();
            _instance.currentRoom.Room.Exit(_instance.previousRoomSleepDelay);
            _instance.UnloadNeighbors(_instance.currentRoom, dirOfNewRoom); // TODO: async?
            _instance.LoadNeighbors(newRoom, dirOfNewRoom.Inverse()); // TODO: async?
            _instance.currentRoom = newRoom;
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
                {
                    var index = newRoomNode.Index + dir.ToVector();
                    var room = GetRoom(index);
                    room.Node = new RoomNode(room, index);
                    room.Node[dir.Inverse()] = newRoomNode;
                    newRoomNode[dir] = room.Node;
                    continue;
                }

                if (neighborNode.Room != null)
                {
                    var index = _roomPool.FindIndex(room => room == neighborNode.Room);
                    RemoveAndReplaceFromPool(index);
                    continue;
                }

                neighborNode.Room = GetRoom(neighborNode.Index, neighborNode);
                neighborNode[dir.Inverse()] = newRoomNode;
                newRoomNode[dir] = neighborNode;
            }
        }

        private Room GetRoom(Vector2Int index, RoomNode roomNode = null)
        {
            Room room;
            if (_roomPool.Count == 0)
            {
                room = Instantiate(roomPrefab, GetPosition(index), Quaternion.identity, transform).GetComponent<Room>();
                room.Node = roomNode;
                return room;
            }

            room = PopFromPool();
            room.Node.Room = null;
            room.Node = roomNode;
            room.transform.position = GetPosition(index);
            return room;
        }

        private Vector3 GetPosition(Vector2Int roomIndex)
        {
            return new Vector3(roomIndex.x * (roomWidth - roomVerticalIntersection),
                roomIndex.y * (roomHeight - roomHorizontalIntersection));
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

        #endregion

        #region Classes

        private class DoubleRoomManagerException : Exception
        {
        }

        #endregion
    }
}