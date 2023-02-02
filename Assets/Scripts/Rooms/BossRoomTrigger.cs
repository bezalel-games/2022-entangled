using UnityEngine;

namespace Rooms
{
    public class BossRoomTrigger : MonoBehaviour
    {
        private const string MINIMAP_PATH = "/UI/Minimap";

        private void OnEnable()
        {
            GameObject.Find(MINIMAP_PATH).SetActive(false);
        }
    }
}