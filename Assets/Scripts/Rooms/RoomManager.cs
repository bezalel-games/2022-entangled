using System;
using UnityEngine;

namespace Rooms
{
    public class RoomManager : MonoBehaviour
    {
        private static RoomManager _instance;
        
        private RoomNode _currentRoom;
        public static RoomNode CurrentRoom
        {
            set
            {
                _instance._currentRoom = value;
                _instance.LoadNeighbors();
            }
        }

        private class DoubleRoomManagerException : Exception{}
        private void Awake()
        {
            if (_instance != null)
                throw new DoubleRoomManagerException();
            _instance = this;
        } 

        private void LoadNeighbors()
        {
            
        }
        
        private void CreateNeighbor()
        {
            
        }
    }
}