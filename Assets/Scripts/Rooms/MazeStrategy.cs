using System;
using System.Collections.Generic;
using Rooms.CardinalDirections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Rooms
{
    public class MazeStrategy : INeighborsStrategy
    {
        #region Fields

        private readonly int _maxDistanceToBoss;
        private readonly int _minDistanceToBoss;
        private readonly float _totalRooms; // number of rooms will be (maxDistance * roomPopulation)

        private Vector2Int _bossIndex;

        private HashSet<Vector2Int> _rooms;

        #endregion
        
        #region Public Methods
        
        public MazeStrategy(int minDistanceToBoss, int maxDistanceToBoss, int totalRooms)
        {
            _minDistanceToBoss = minDistanceToBoss;
            _maxDistanceToBoss = maxDistanceToBoss;
            
            _totalRooms = Math.Min(Math.Max(totalRooms, maxDistanceToBoss), maxDistanceToBoss*maxDistanceToBoss);

            _rooms = new HashSet<Vector2Int>(); 
            CreateMaze();
        }
        
        public bool RoomExists(Vector2Int index)
        {
            return _rooms.Contains(index);
        }

        public bool IsBossRoom(Vector2Int index)
        {
            return index == _bossIndex;
        }

        #endregion
        
        #region Private Methods
        
        private void CreateMaze()
        {
            _rooms.Add(Vector2Int.zero);

            CreatePathToBoss();
            PopulateMaze();
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
                _rooms.Add(currRoom);
                
                foreach (Direction dir in DirectionExt.GetDirections())
                {
                    var newRoom = currRoom + dir.ToVector();
                    if(queued.Contains(newRoom) || 
                       Vector2Int.Distance(Vector2Int.zero, newRoom) > _maxDistanceToBoss) continue;

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
            var row = Random.Range(0, _maxDistanceToBoss + 1);
            var col = Random.Range(_minDistanceToBoss - row, _maxDistanceToBoss - row);

            row *= (Random.value > 0.5) ? 1 : -1;
            col *= (Random.value > 0.5) ? 1 : -1;
            
            _bossIndex = new Vector2Int(col, row);
            
            
            Vector2Int currRoom = Vector2Int.zero;
            int roomCount = 0;
            Vector2Int lastDir = Vector2Int.zero;

            while (currRoom != _bossIndex)
            {
                int roomsLeft = _maxDistanceToBoss - roomCount;
                int distance = (int) Vector2Int.Distance(currRoom, _bossIndex);

                if (distance > roomsLeft)
                {
                    throw new Exception(
                        "Got too far from bossRoom. this shouldn't happen, something is wrong with the algorithm");
                }
                
                //if needed create shortest path
                if (distance == roomsLeft)
                {
                    CreateStraightPath(currRoom, _bossIndex);
                    break;
                }

                // --------------- create random path ----------------------------
                //Get all possible directions
                var possible = new List<Vector2Int>();
                foreach (Direction dir in DirectionExt.GetDirections())
                {
                    Vector2Int newCurr = currRoom + dir.ToVector();
                    if (_rooms.Contains(newCurr) ||
                        Vector2Int.Distance(newCurr, _bossIndex) > _maxDistanceToBoss) continue;
                    
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

                    _rooms.Add(currRoom);
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
            _rooms.Add(curr);

            Vector2Int yDir = Math.Sign(to.y - from.y) * Vector2Int.up;
            Vector2Int xDir = Math.Sign(to.x - from.x) * Vector2Int.right;
            
            while (curr != to)
            {
                var dir = Vector2Int.zero;
                if(curr.x == to.x)
                    dir =  yDir;
                else if(curr.y == to.y)
                    dir = xDir;
                else
                    dir = (Random.value > 0.5f) ? yDir : xDir;

                curr += dir;
                _rooms.Add(curr);
            }
        }

        #endregion
    }
}