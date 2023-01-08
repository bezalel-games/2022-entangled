using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Enemies;
using Rooms.CardinalDirections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Rooms
{
    public partial class Room : MonoBehaviourExt
    {
        #region Serialized Fields

        [SerializeField] private int _inPriority;
        [SerializeField] private int _outPriority;
        [SerializeField] private CinemachineVirtualCamera _vCam;
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private Tilemap _frameTilemap;
        [SerializeField] private float _doorAnimationDuration = 0.5f;

        [SerializeField] private GameObject _miniMap;

        #endregion

        #region Non-Serialized Fields

        private Collider2D _collider;
        private Coroutine _sleepCoroutine;
        private GateState _gateState = GateState.OPEN;

        #endregion

        #region Properties

        [field: SerializeField] public GameObject RoomContent { get; private set; }
        [field: SerializeField] public RoomEnemies Enemies { get; private set; }
        [field: SerializeField] public RoomNode Node { get; set; }

        public bool GateClosed
        {
            set { GateState = value ? GateState.CLOSING : GateState.OPENING; }
        }

        private GateState GateState
        {
            get => _gateState;
            set
            {
                if (_gateState == value) return;
                _gateState = value;
                var tile = value switch
                {
                    GateState.CLOSING => RoomManager.RoomProperties.ClosingGateTile,
                    GateState.CLOSED => RoomManager.RoomProperties.ClosedGateTile,
                    GateState.OPENING => RoomManager.RoomProperties.OpeningGateTile,
                    GateState.OPEN => RoomManager.RoomProperties.OpenedGateTile,
                    _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
                };
                foreach (var gateTilePos in RoomManager.RoomProperties.GatePositions)
                {
                    _tilemap.SetTile(gateTilePos, tile);
                }

                if (value is GateState.CLOSING or GateState.OPENING)
                    DelayInvoke(() => GateState = value + 1, _doorAnimationDuration);
            }
        }

        #endregion

        #region Function Events

        private void OnTriggerEnter2D(Collider2D col)
        {
            RoomManager.EnteredRoom(Node);
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            RoomManager.ExitedRoom(Node, col.gameObject);
        }

        #endregion

        #region Public Methods

        public void Enter()
        {
            if (_sleepCoroutine != null)
            {
                StopCoroutine(_sleepCoroutine);
                _sleepCoroutine = null;
            }

            _vCam.Priority = _inPriority;
            RoomContent.SetActive(true);
            Enemies.Node = Node;
            if (!Node.Cleared)
            {
                Enemies.Activate();
                GateClosed = true;
            }
        }

        public void Exit(float sleepDelay = 0)
        {
            _vCam.Priority = _outPriority;
            if (sleepDelay <= 0)
                RoomContent.SetActive(false);
            else
                _sleepCoroutine = StartCoroutine(SleepWithDelay(sleepDelay));
        }

        public void ShowDoor(Direction dir, bool show = true)
        {
            foreach (var gateTilePos in RoomManager.RoomProperties.GatePositions)
            {
                var vec2 = new Vector2Int(gateTilePos.x, gateTilePos.y);
                if (vec2.ToDirectionRounded() == dir)
                {
                    var tile = show ? RoomManager.RoomProperties.GateFrameTile : RoomManager.RoomProperties.WallTile;
                    _frameTilemap.SetTile(gateTilePos, tile);
                }
            }
        }

        public void ShowOnMiniMap()
        {
            if (!MinimapManager.HasRoom(Node.Index))
                Instantiate(MinimapManager.MinimapRoomPrefab, transform);
        }

        #endregion

        #region Private Methods

        private IEnumerator SleepWithDelay(float sleepDelay)
        {
            yield return new WaitForSeconds(sleepDelay);
            RoomContent.SetActive(false);
            _sleepCoroutine = null;
        }

        #endregion
    }
}

public enum GateState
{
    CLOSING,
    CLOSED,
    OPENING,
    OPEN
}