using System.Collections;
using Cinemachine;
using UnityEngine;

namespace Rooms
{
    public class Room : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private int inPriority;
        [SerializeField] private int outPriority;
        [SerializeField] private CinemachineVirtualCamera vCam;
        [SerializeField] private GameObject roomContent;

        #endregion

        #region Non-Serialized Fields

        private Collider2D _collider;

        #endregion

        #region Properties

        public RoomNode Node { get; set; }

        #endregion

        #region Function Events

        private void OnTriggerEnter2D(Collider2D col)
        {
            RoomManager.ChangeRoom(Node);
        }

        #endregion

        #region Public Methods

        public void Enter()
        {
            vCam.Priority = inPriority;
            roomContent.SetActive(true);
        }

        public void Exit(float sleepDelay = 0)
        {
            vCam.Priority = outPriority;
            if (sleepDelay <= 0)
                roomContent.SetActive(false);
            else
                StartCoroutine(SleepWithDelay(sleepDelay));
        }

        #endregion

        #region Private Methods

        private IEnumerator SleepWithDelay(float sleepDelay)
        {
            yield return new WaitForSeconds(sleepDelay);
            roomContent.SetActive(false);
        }

        #endregion
    }
}