﻿using System;
using System.Collections;
using Cinemachine;
using Enemies;
using Interactables;
using Rooms.CardinalDirections;
using TMPro;
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
        [SerializeField] private float _doorOpeningAnimationDuration = 1;
        [SerializeField] private float _doorClosingAnimationDuration = 1;

        [SerializeField] private GameObject _miniMap;

        #endregion

        #region Non-Serialized Fields

        private Collider2D _collider;
        private Coroutine _sleepCoroutine;
        private GateState _gateState = GateState.OPEN;

        #endregion

        #region Properties

        public CinemachineBasicMultiChannelPerlin ChannelPerlin { get; private set; }
        [field: SerializeField] public GameObject RoomContent { get; private set; }
        [field: SerializeField] public RoomEnemies Enemies { get; private set; }
        [field: SerializeField] public RoomNode Node { get; set; }
        [field: SerializeField] private RoomProperties RoomProperties { get; set; }
        
        [field: SerializeField] public TextMeshProUGUI TutorialTxt { get; private set; }
        [field: SerializeField] public Transform TutorialEnemyPosition { get; private set; }

        public Interactable Interactable { get; private set; }

        public bool GateClosed
        {
            set
            {
                GateState = value ? GateState.CLOSING : GateState.OPENING;
                _tilemap.RefreshAllTiles();
            }
        }

        private GateState GateState
        {
            get => _gateState;
            set
            {
                if (_gateState == value || RoomManager.IsTutorial) return;
                _gateState = value;
                var tile = value switch
                {
                    GateState.CLOSING => RoomProperties.ClosingGateTile,
                    GateState.CLOSED => RoomProperties.ClosedGateTile,
                    GateState.OPENING => RoomProperties.OpeningGateTile,
                    GateState.OPEN => RoomProperties.OpenedGateTile,
                    _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
                };
                foreach (var gateTilePos in RoomProperties.GatePositions)
                {
                    _tilemap.SetTile(gateTilePos, tile);
                }

                if (value is GateState.CLOSING)
                    DelayInvoke(() => GateState = GateState.CLOSED, _doorClosingAnimationDuration);
                else if (value is GateState.OPENING)
                    DelayInvoke(() => GateState = GateState.OPEN, _doorOpeningAnimationDuration);
            }
        }

        #endregion

        #region Function Events

        private void Start()
        {
            ChannelPerlin = _vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

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

            if (!RoomManager.Nodes.ContainsKey(Node.Index))
                RoomManager.Nodes[Node.Index] = Node;

            if (RoomManager.CameraPerlin != null)
                RoomManager.CameraPerlin.m_AmplitudeGain = 0;

            RoomManager.CameraPerlin = ChannelPerlin;
            _vCam.Priority = _inPriority;
            RoomContent.SetActive(true);
            Enemies.Node = Node;
            if (RoomManager.IsTutorial)
            {
                CreateTutorialWalls();
            }
            else
            {
                if (!Node.Cleared)
                {
                    Enemies.Activate();
                    GateClosed = true;
                }
            }
            _tilemap.RefreshAllTiles();
        }

        public void Clean()
        {
            Enemies.RemoveEnemies();
            RemoveInteractable();
        }

        public void Exit(float sleepDelay = 0)
        {
            _vCam.Priority = _outPriority;
            if (sleepDelay <= 0)
                RoomContent.SetActive(false);
            else
                _sleepCoroutine = StartCoroutine(SleepWithDelay(sleepDelay));
        }

        public void ShowDoor(Direction dir, bool show = true, Vector2Int index = default)
        {
            if (RoomManager.IsTutorial) return;
            foreach (var gateTilePos in RoomProperties.GatePositions)
            {
                if (((Vector2Int) gateTilePos).ToDirectionRounded() == dir)
                {
                    var tile = show ? RoomProperties.GateFrameTile : RoomProperties.WallTile;
                    _frameTilemap.SetTile(gateTilePos, tile);
                }
            }
        }

        public void InitInteractable(RoomType type)
        {
            if (Interactable != null)
                Interactable.gameObject.SetActive(true);
            else
                Interactable = Instantiate(RoomManager.Interactables[type], transform);

            Interactable.ParentNode = Node;
            if (Node.Interacted)
            {
                Debug.Log("call from Room:155");
                Interactable.SetToAfter();
            }
        }

        #endregion

        #region Private Methods

        private void CreateTutorialWalls()
        {
            foreach (var gateTilePos in RoomProperties.GatePositions)
            {
                var tile = RoomProperties.GroundTile;
                if (gateTilePos.x == -(RoomProperties.Width / 2)
                    || gateTilePos.x == (RoomProperties.Width / 2) - 1)
                {
                    tile = RoomProperties.WallTile;
                }

                var foreGround = RoomProperties.EmptyTile;
                if (Node.Index.y == 0)
                {
                    if (gateTilePos.y <= -(RoomProperties.Height / 2 - 1))
                        tile = RoomProperties.WallTile;
                }

                if (Node.Index.y == RoomManager.TutorialLength - 1)
                {
                    if (gateTilePos.y == RoomProperties.Height / 2)
                    {
                        if (gateTilePos.x is -1 or 0)
                        {
                            tile = RoomProperties.OpenedGateTile;
                            foreGround = RoomProperties.GateFrameTile;
                        }
                        else
                        {
                            tile = RoomProperties.WallTile;
                        }
                    }
                }

                _tilemap.SetTile(gateTilePos, tile);
                _frameTilemap.SetTile(gateTilePos, foreGround);
            }
        }

        private IEnumerator SleepWithDelay(float sleepDelay)
        {
            yield return new WaitForSeconds(sleepDelay);
            RoomContent.SetActive(false);
            _sleepCoroutine = null;
        }

        private void RemoveInteractable()
        {
            if (Interactable != null)
            {
                Destroy(Interactable.gameObject);
                Interactable = null;
            }
        }

        #endregion
    }

    public enum RoomType
    {
        MONSTERS,
        BOSS,
        TREASURE,
        FOUNTAIN,
        START,
        NONE,
        TUTORIAL
    }
}

public enum GateState
{
    CLOSING,
    CLOSED,
    OPENING,
    OPEN
}