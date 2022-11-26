using UnityEngine;

namespace Rooms
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private RoomNode node;

        #region Function Events
        
        private void OnBecameVisible() //should be replaced
        {
            RoomManager.CurrentRoom = node;
        }
        
        #endregion

        #region Private Methods
        
        
        
        #endregion
    }
}