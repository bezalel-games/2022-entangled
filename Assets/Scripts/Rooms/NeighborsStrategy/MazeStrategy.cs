using System;
using System.Collections.Generic;
using Rooms.CardinalDirections;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Rooms.NeighborsStrategy
{
    public class MazeStrategy : INeighborsStrategy
    {
        #region Fields

        private readonly int _maxDistanceToBoss;
        private readonly int _minDistanceToBoss;
        private readonly int _totalRooms; // number of rooms will be (maxDistance * roomPopulation)

        private Vector2Int _bossIndex;

        private readonly Dictionary<Vector2Int, RoomType> _rooms;

        private readonly int _fountainDistance;
        private readonly int _treasureDistance;
        private int _fountainCount;
        private int _treasureCount;

        #endregion

        #region Construcors

        public MazeStrategy(int minDistanceToBoss, int maxDistanceToBoss, int totalRooms, int numFountains = 0,
            int numTreasure = 0)
        {
            _minDistanceToBoss = minDistanceToBoss;
            _maxDistanceToBoss = maxDistanceToBoss;

            _totalRooms = (int)Math.Min(Math.Max(totalRooms, maxDistanceToBoss),
                Mathf.Pow(2 * maxDistanceToBoss + 1, 2));

            _fountainDistance =
                (int)Mathf.Ceil(((float)_totalRooms / ((numFountains + 1) * _totalRooms)) * _maxDistanceToBoss);
            _treasureDistance =
                (int)Mathf.Ceil(((float)_totalRooms / ((numTreasure + 1) * _totalRooms)) * _maxDistanceToBoss);

            _rooms = new();
            CreateMaze();
        }

        #endregion

        #region INeighborsStrategy Implementation

        public RoomType RoomType(Vector2Int index) => _rooms.ContainsKey(index) ? _rooms[index] : Rooms.RoomType.NONE;

        public int RoomRank(int minRoomRank, Vector2Int index, AnimationCurve distanceToRankFunction)
        {
            return minRoomRank +
                   (int)(distanceToRankFunction.Evaluate(index.L1Norm() / (float)_maxDistanceToBoss) * minRoomRank);
        }

        #endregion

        #region Private Methods

        private void PrintMaze()
        {
            string s = "";
            for (int row = _maxDistanceToBoss; row >= -_maxDistanceToBoss; row--)
            {
                for (int col = -_maxDistanceToBoss; col <= _maxDistanceToBoss; col++)
                {
                    var room = new Vector2Int(col, row);
                    if (!_rooms.ContainsKey(room))
                        s += " - ";
                    else
                    {
                        s += _rooms[room] switch
                        {
                            Rooms.RoomType.START => " S ",
                            Rooms.RoomType.BOSS => " B ",
                            Rooms.RoomType.TREASURE => " T ",
                            Rooms.RoomType.FOUNTAIN => " F ",
                            Rooms.RoomType.MONSTERS => " m ",
                            _ => " - "
                        };
                    }
                }

                s += "\n";
            }

            Debug.Log(s);
        }

        private void CreateMaze()
        {
            AddRoom(Vector2Int.zero);

            CreatePathToBoss();
            PopulateMaze();

            PrintMaze();
        }

        //randomly add rooms to Set (using BFS to make sure they're all reachable)
        private void PopulateMaze()
        {
            Queue<Vector2Int> roomsToAdd = new Queue<Vector2Int>();
            HashSet<Vector2Int> queued = new HashSet<Vector2Int>();

            roomsToAdd.Enqueue(Vector2Int.zero);
            queued.Add(Vector2Int.zero);

            //BFS until we populate enough rooms
            while (_rooms.Count < _totalRooms)
            {
                if (roomsToAdd.Count == 0)
                {
                    roomsToAdd.Enqueue(Vector2Int.zero);
                    queued.Clear();
                    queued.Add(Vector2Int.zero);
                }

                var currRoom = roomsToAdd.Dequeue();
                AddRoom(currRoom);

                foreach (Direction dir in DirectionExt.GetDirections())
                {
                    var newRoom = currRoom + dir.ToVector();
                    if (queued.Contains(newRoom) ||
                        Vector2Ext.L1Distance(Vector2Int.zero, newRoom) > _maxDistanceToBoss) continue;

                    /*
                     * might prefer to try 'if (Random.value > 0.5f || _rooms.Contains(newRoom))' for faster generation 
                     */
                    if (Random.value > 0.5f)
                    {
                        roomsToAdd.Enqueue(newRoom);
                        queued.Add(newRoom);
                    }
                }
            }
        }

        private void CreatePathToBoss()
        {
            /*
             * minDistance < row+col < maxDistance -> minDistance-row < col < maxDistance-row
             */
            var row = Random.Range(0, _maxDistanceToBoss);
            var col = Random.Range(_minDistanceToBoss - row, _maxDistanceToBoss - row);

            row *= (Random.value > 0.5) ? 1 : -1;
            col *= (Random.value > 0.5) ? 1 : -1;

            _bossIndex = new Vector2Int(col, row);
            AddRoom(_bossIndex);

            Vector2Int currRoom = _bossIndex + Vector2Int.down;
            AddRoom(currRoom);
            int roomCount = 1;
            Vector2Int lastDir = Vector2Int.zero;

            while (currRoom != Vector2Int.zero)
            {
                int roomsLeft = _maxDistanceToBoss - roomCount;
                int distance = Vector2Ext.L1Norm(currRoom);

                if (distance > roomsLeft + 1)
                {
                    PrintMaze();
                    Debug.Log($"Distance: {distance}, RoomsLeft: {roomsLeft}");
                    throw new Exception(
                        "Got too far from start. this shouldn't happen, something is wrong with the algorithm");
                }

                //if needed create shortest path
                if (distance >= roomsLeft)
                {
                    CreateStraightPath(currRoom, Vector2Int.zero);
                    break;
                }

                // --------------- create random path ----------------------------
                //Get all possible directions
                var possible = new List<Vector2Int>();
                foreach (Direction dir in DirectionExt.GetDirections())
                {
                    Vector2Int newCurr = currRoom + dir.ToVector();
                    if (_rooms.ContainsKey(newCurr) ||
                        Vector2Ext.L1Distance(newCurr, Vector2Int.zero) > roomsLeft - 1) continue;

                    possible.Add(dir.ToVector());
                }

                //If no direction is possible, go back one room
                if (possible.Count == 0)
                {
                    currRoom -= lastDir;
                    roomCount--;
                }
                //else, pick random direction and move in it
                else
                {
                    Vector2Int moveDir = possible[Random.Range(0, possible.Count)];
                    currRoom += moveDir;

                    AddRoom(currRoom);
                    roomCount++;
                }
            }
        }

        /**
         * Adds to _rooms a shortest path from 'from' to 'to'. i.e., for path p it holds |p|=Distance(from,to).
         */
        private void CreateStraightPath(Vector2Int from, Vector2Int to)
        {
            Vector2Int curr = from;
            AddRoom(curr);

            Vector2Int yDir = Math.Sign(to.y - from.y) * Vector2Int.up;
            Vector2Int xDir = Math.Sign(to.x - from.x) * Vector2Int.right;

            while (curr != to)
            {
                var dir = Vector2Int.zero;
                if (curr.x == to.x)
                    dir = yDir;
                else if (curr.y == to.y)
                    dir = xDir;
                else
                    dir = (Random.value > 0.5f) ? yDir : xDir;

                curr += dir;
                AddRoom(curr);
            }
        }

        private void AddRoom(Vector2Int index)
        {
            if (_rooms.ContainsKey(index))
                return;

            _rooms[index] = GenerateType(index);
        }

        private RoomType GenerateType(Vector2Int index)
        {
            if (index == Vector2Int.zero)
                return Rooms.RoomType.START;
            if (index == _bossIndex)
                return Rooms.RoomType.BOSS;

            var dist = index.L1Norm(); // equivalent to L1Distance(zero, index)
            if (_fountainDistance > 0)
            {
                var poss = _rooms.Count / (_fountainDistance * (_fountainCount + 1) + _fountainDistance);
                if (dist / _fountainDistance > _fountainCount && Random.value < poss)
                {
                    _fountainCount++;
                    return Rooms.RoomType.FOUNTAIN;
                }
            }

            if (_treasureDistance > 0)
            {
                var poss = _rooms.Count / (_treasureDistance * (_treasureCount + 1) + _treasureDistance);
                if (dist / _treasureDistance > _treasureCount && Random.value < poss)
                {
                    _treasureCount++;
                    return Rooms.RoomType.TREASURE;
                }
            }

            return Rooms.RoomType.MONSTERS;
        }

        #endregion
    }
}