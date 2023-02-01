using System;
using System.Collections.Generic;
using Audio;
using Cards;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;
using Rooms.CardinalDirections;
using Rooms.NeighborsStrategy;
using Direction = Rooms.CardinalDirections.Direction;
using Enemies;
using Interactables;
using Managers;
using Player;
using Utils;

namespace Rooms
{
    public class RoomManager : MonoBehaviour
    {
        #region Serialized Fields

        [Tooltip("The Room Component of the Room Prefab used for spawning all rooms")] [SerializeField]
        private Room _roomPrefab;

        [Tooltip("Time before the previous room is put to sleep after a room transition")] [SerializeField]
        private float _previousRoomSleepDelay = 1;

        [Tooltip("A scriptable object containing all the enemies to spawn in the game")] [SerializeField]
        private EnemyDictionary _enemyDictionary;


        [SerializeField] private RoomProperties _roomProperties;
        [SerializeField] private bool _spawnEnemies = true;
        [SerializeField] [Range(0f, 1f)] private float _ghostChanceFactor = 0.66f;
        [SerializeField] private InteractablePair[] _interactablePairs;
        [SerializeField] private ParticleSystemForceField _bossRoomForceFieldPrefab;
        [SerializeField] private ParticleSystem _bossRoomParticlesPrefab;

        [field: Header("Play mode")] [SerializeField]
        private NeighborsStrategy _playMode = NeighborsStrategy.MAZE;

        [Header("Maze settings")] [SerializeField]
        private int _minDistanceFromBoss = 5;

        [SerializeField] private int _maxDistanceFromBoss = 6;
        [SerializeField] private int _totalNumberOfRooms = 40;
        [SerializeField] private int _fountainCount = 1;
        [SerializeField] private int _treasureCount = 2;

        [Header("Boss room")] [SerializeField] private Room _bossRoomPrefab;

        [Header("Tutorial Settings")] [SerializeField]
        private List<TutorialRoomProperties> _tutorialRooms;

        [Header("Room rank function")] 
        [SerializeField] private int _minDistanceForGhost = 3;
        [SerializeField] private int _minRoomRank = 20;
        [SerializeField] private int _maxRoomRank = 50;

        [SerializeField] private AnimationCurve _distanceToRankFunction;

        #endregion

        #region Non-Serialized Fields

        private RoomNode _currentRoom;
        private static RoomManager _instance;
        private readonly List<Room> _roomPool = new();

        private CinemachineBasicMultiChannelPerlin _cameraPerlin;
        private RoomNode _nextRoom;
        private INeighborsStrategy _strategy;
        private readonly Dictionary<Vector2Int, RoomNode> _nodes = new();
        private readonly Dictionary<RoomType, Interactable> _interactables = new();
        private ParticleSystem _bossRoomParticles;
        private ParticleSystemForceField _bossRoomParticlesForceField;

        #endregion

        #region Properties

        public static Vector2Int CurrentRoomIndex => _instance == null ? Vector2Int.zero : _instance._currentRoom.Index;
        public static bool IsTutorial => _instance._playMode == NeighborsStrategy.TUTORIAL;
        public static Dictionary<Vector2Int, RoomNode> Nodes => _instance._nodes;
        public static Dictionary<RoomType, Interactable> Interactables => _instance._interactables;

        public static CinemachineBasicMultiChannelPerlin CameraPerlin
        {
            get => _instance._cameraPerlin;
            set => _instance._cameraPerlin = value;
        }

        public static EnemyDictionary EnemyDictionary => _instance._enemyDictionary;

        private int ActualHalfWidth => _roomProperties.Width / 2 - _roomProperties.WallSize;
        private int ActualHalfHeight => _roomProperties.Height / 2 - _roomProperties.WallSize;

        public static int TutorialLength => _instance._tutorialRooms.Count;

        #endregion

        #region Events

        public static event Action<float> RoomChanged;

        #endregion

        #region Function Events

        private void Awake()
        {
            if (_instance != null)
                throw new DoubleRoomManagerException();
            _instance = this;
            GameManager.FinishedCurrentRoom += FinishedCurrentRoom;

            foreach (InteractablePair pair in _interactablePairs)
            {
                Interactables[pair.RoomType] = pair.Interactable;
            }

            InitStrategy();
        }

        private void Start()
        {
            _enemyDictionary = Instantiate(_enemyDictionary); // duplicate to not overwrite the saved asset
            var firstRoomIndex = Vector2Int.zero;
            _currentRoom = CreateNode(firstRoomIndex, null);
            _currentRoom.Room = GetRoom(_currentRoom.Index, _currentRoom);
            _currentRoom.Cleared = true;
            _currentRoom.Room.Enter();

            if (IsTutorial)
                FillRoom(_currentRoom);

            if (_playMode is NeighborsStrategy.MAZE or NeighborsStrategy.TUTORIAL)
                RepositionParticles(_currentRoom);

            LoadNeighbors(_currentRoom);
            InitContentInNeighbors();

            MinimapManager.AddRoom(Vector2Int.zero);
            RoomChanged?.Invoke(_instance._currentRoom.Intensity);

            AudioManager.SetMusic(IsTutorial ? MusicSounds.TUTORIAL : MusicSounds.RUN);

            if (_playMode is NeighborsStrategy.MAZE or NeighborsStrategy.ENDLESS)
            {
                GameManager.PlayerController.DelayInvoke((
                    GameManager.ShowCards), 1f);
            }
        }

        private void OnDestroy()
        {
            GameManager.FinishedCurrentRoom -= FinishedCurrentRoom;
        }

        #endregion

        #region Public Method

        public static float GetGhostChance(Vector2Int index)
        {
            if (index.L1Norm() < _instance._minDistanceForGhost) return 0;
            
            return _instance._strategy.GhostChance(
                _instance._minRoomRank,
                _instance._maxRoomRank,
                index,
                _instance._distanceToRankFunction,
                _instance._ghostChanceFactor
            );
        }

        public static void EnteredRoom(RoomNode roomNode)
        {
            if (IsTutorial)
            {
                if (roomNode.Index.y == TutorialLength)
                {
                    LoadManager.LoadRun();
                    return;
                }
            }

            _instance._nextRoom = roomNode;
        }

        public static void ExitedRoom(RoomNode roomNode, GameObject exitingObject)
        {
            if (_instance._nextRoom == null) return;
            if (_instance._currentRoom.Index == roomNode.Index)
                ChangeRoom(_instance._nextRoom, exitingObject);
            _instance._nextRoom = null;
        }

        public static void RepositionRoom(Room room)
        {
            room.transform.position = _instance.GetPosition_Inner(room.Node.Index);
            room.UpdateDoors();
            room.Clean();

            _instance.FillRoom(room.Node);
        }

        public static Vector3 GetPosition(Vector2Int index) => _instance.GetPosition_Inner(index);

        public static RoomType GetRoomType(Vector2Int index) => _instance._strategy.RoomType(index);

        #endregion

        #region Private Methods

        private static void ChangeRoom(RoomNode newRoom, GameObject transitioningObject)
        {
            if (newRoom == _instance._currentRoom) return;
            var indexDiff = newRoom.Index - _instance._currentRoom.Index;
            var dirOfNewRoom = indexDiff.ToDirection();
            var player = transitioningObject.GetComponent<PlayerController>();
            if (player)
                MovePlayerToNewRoom(newRoom.Index, dirOfNewRoom, (Vector2) indexDiff, player);

            var previousRoom = _instance._currentRoom;
            _instance._currentRoom = newRoom;

            newRoom.Room.Enter();
            previousRoom.Room.Exit(_instance._previousRoomSleepDelay);

            _instance.UnloadNeighbors(previousRoom, dirOfNewRoom); // TODO: async?
            _instance.LoadNeighbors(newRoom, dirOfNewRoom.Inverse()); // TODO: async?
            MinimapManager.AddRoom(newRoom.Index);
            if (newRoom.Cleared)
                InitContentInNeighbors();
            RoomChanged?.Invoke(_instance._currentRoom.Intensity);

            if (_instance._playMode is NeighborsStrategy.MAZE or NeighborsStrategy.TUTORIAL)
            {
                if (_instance._playMode is NeighborsStrategy.MAZE 
                    && _instance._strategy.RoomType(newRoom.Index) == RoomType.BOSS)
                    _instance._bossRoomParticlesForceField.transform.position =
                        GetPosition(newRoom.Index + Vector2Int.up * 3);
                
                _instance.RepositionParticles(newRoom);
            }

            if (_instance._strategy.RoomType(newRoom.Index) == RoomType.BOSS)
                AudioManager.SetMusic(MusicSounds.BOSS1);
        }

        private void RepositionParticles(RoomNode node)
        {
            if (_bossRoomParticles == null
                || (_playMode != NeighborsStrategy.MAZE && _playMode != NeighborsStrategy.TUTORIAL)) return;

            var bossIndex = _playMode == NeighborsStrategy.MAZE
                ? ((MazeStrategy) _strategy).GetBossRoom()
                : Vector2Int.up * TutorialLength;

            var delta = node.Index - bossIndex;
            delta.x = (int) (Math.Sign(delta.x) * _roomProperties.Width * 0.75f);
            delta.y = (int) (Math.Sign(delta.y) * _roomProperties.Height * 0.75f);

            _bossRoomParticles.transform.position = GetPosition(node.Index) + new Vector3(delta.x, delta.y, 0);
            _bossRoomParticles.transform.rotation = Quaternion.Euler(0, 0, Vector2Ext.Angles(Vector2.up, delta));
        }

        private static void MovePlayerToNewRoom(Vector2Int newRoomIndex, Direction dirOfNewRoom, Vector3 walkDirection,
            PlayerController player)
        {
            var nextRoomPosition = _instance.GetPosition_Inner(newRoomIndex);
            float threshold = dirOfNewRoom switch
            {
                Direction.WEST => nextRoomPosition.x + _instance.ActualHalfWidth,
                Direction.EAST => nextRoomPosition.x - _instance.ActualHalfWidth,
                Direction.SOUTH => nextRoomPosition.y + _instance.ActualHalfHeight,
                Direction.NORTH => nextRoomPosition.y - _instance.ActualHalfHeight,
                _ => throw new ArgumentOutOfRangeException()
            };
            player.OverrideMovement(walkDirection.normalized, threshold);
        }

        private void UnloadNeighbors(RoomNode prevRoom, Direction? dirOfNewRoom = null)
        {
            foreach (Direction dir in DirectionExt.GetDirections())
            {
                if (dir == dirOfNewRoom || prevRoom[dir] == null)
                    continue;

                var neighbor = prevRoom[dir].Room;
                // neighbor.GateClosed = false;

                // Don't add room to pool if not existing or if boss room
                if (_strategy.RoomType(prevRoom.Index + dir.ToVector()) is RoomType.NONE or RoomType.BOSS)
                    continue;

                _roomPool.Add(neighbor);
            }
        }

        private void LoadNeighbors(RoomNode newRoomNode, Direction? dirOfOldRoom = null)
        {
            foreach (Direction dir in DirectionExt.GetDirections())
            {
                if (dir == dirOfOldRoom)
                    continue;
                var roomIndex = newRoomNode.Index + dir.ToVector();
                var roomType = _strategy.RoomType(roomIndex);
                if (roomType is RoomType.NONE || (roomType is RoomType.BOSS && dir is not Direction.NORTH))
                {
                    newRoomNode[dir] = null;
                    continue;
                }

                var neighborNode = Nodes.ContainsKey(roomIndex) ? Nodes[roomIndex] : null;
                if (neighborNode == null)
                    // no neighbor node yet
                {
                    var isBossRoom = roomType is RoomType.BOSS;
                    var room = GetRoom(roomIndex, isBossRoom: isBossRoom);
                    room.Node = CreateNode(roomIndex, room);
                    room.Node[dir.Inverse()] = newRoomNode;
                    newRoomNode[dir] = room.Node;
                    continue;
                }

                if (neighborNode.Room != null && neighborNode.Room.Node == neighborNode)
                    // neighbor node exists and still has a room that is linked to it
                {
                    var poolIndex = _roomPool.FindIndex(room => room == neighborNode.Room);
                    if (poolIndex >= 0 && poolIndex < _roomPool.Count)
                    {
                        RemoveAndReplaceFromPool(poolIndex);
                        continue;
                    }
                }

                // neighbor node exists but needs a new room
                var neighborRoom = GetRoom(neighborNode.Index, neighborNode);
                neighborNode.Room = neighborRoom;
                neighborNode[dir.Inverse()] = newRoomNode;
                newRoomNode[dir] = neighborNode;
            }
        }

        private int RoomRank(Vector2Int index) =>
            _strategy.RoomRank(_minRoomRank, _maxRoomRank, index, _distanceToRankFunction);

        private RoomNode CreateNode(Vector2Int index, Room room) =>
            new(room, index, RoomRank(index), _strategy.RoomIntensity(index));

        private void FillTutorialRoom(RoomNode roomNode)
        {
            if (!IsTutorial || roomNode.Index.y >= TutorialLength) return;

            roomNode.Room.Clean();

            int room = roomNode.Index.y;
            var roomProp = _tutorialRooms[room];

            if (roomProp._sprite != null && roomProp._showSprite)
            {
                roomNode.Room.TutorialSpriteRenderer.sprite = roomProp._sprite;
            }
            else
            {
                roomNode.Room.TutorialTxt.text = roomProp._text;
            }

            foreach (var enemy in roomProp._enemies)
            {
                if (enemy._enemy == null) continue;

                Enemy enemyObj = EnemyDictionary[enemy._enemy.Index].Spawn(
                    roomNode.Room.TutorialEnemyPosition.position,
                    roomNode.Room.Enemies.transform,
                    true).enemy;
                enemyObj.Barrier.Active = enemy._hasBarrier;
            }
        }

        private void SpawnEnemies(RoomNode roomNode)
        {
            if (IsTutorial) return;
            if (!_spawnEnemies || roomNode.Cleared) return;
            roomNode.Room.Clean();

            var roomCenter = GetPosition_Inner(roomNode.Index);
            var enemiesTransform = roomNode.Room.Enemies.transform;
            var numOfEnemyTypes = roomNode.Enemies.Length;
            var ghostChance = GetGhostChance(roomNode.Index);
            for (int enemyType = 0; enemyType < numOfEnemyTypes; ++enemyType)
            for (int i = roomNode.Enemies[enemyType]; i > 0; i--)
                SpawnEnemyInRandomPos(EnemyDictionary[enemyType], roomCenter, enemiesTransform, ghostChance);
        }

        private Room GetRoom(Vector2Int index, RoomNode roomNode = null, bool isBossRoom = false)
        {
            Room room;
            if (isBossRoom)
            {
                room = Instantiate(_bossRoomPrefab, GetPosition_Inner(index), Quaternion.identity, transform);
                room.Node = roomNode;
                return room;
            }

            if (_roomPool.Count == 0)
            {
                room = Instantiate(_roomPrefab, GetPosition_Inner(index), Quaternion.identity, transform);
                room.Node = roomNode;
                return room;
            }

            room = PopFromPool();
            room.Clean();
            room.Node.Room = null;
            room.Node = roomNode;
            room.UpdateDoors();
            room.transform.position = GetPosition_Inner(index);
            return room;
        }

        private Vector3 GetPosition_Inner(Vector2Int roomIndex)
        {
            return new Vector3(roomIndex.x * (_roomProperties.Width - _roomProperties.VerticalIntersection),
                roomIndex.y * (_roomProperties.Height - _roomProperties.HorizontalIntersection));
        }

        private void RemoveAndReplaceFromPool(int index)
        {
            var last = PopFromPool();
            if (index >= _roomPool.Count) return;
            _roomPool[index] = last;
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
            float halfWidth = _roomProperties.Width / 2 - _roomProperties.WallSize - _roomProperties.EnemySpawnMargin;
            float halfHeight = _roomProperties.Height / 2 - _roomProperties.WallSize - _roomProperties.EnemySpawnMargin;
            var x = Random.Range(-halfWidth, halfWidth);
            var y = Random.Range(-halfHeight, halfHeight);
            return new Vector3(x, y);
        }

        private void SpawnEnemyInRandomPos(EnemyDictionary.Entry enemyEntry, Vector3 roomCenter,
            Transform enemiesTransform, float ghostChance = 0)
        {
            for (int i = 0; i < 20; i++)
            {
                if (enemyEntry.Spawn(roomCenter + RandomPosInRoom(), enemiesTransform, ghostChance: ghostChance).success)
                    return;
            }

            enemyEntry.Spawn(roomCenter + RandomPosInRoom(), enemiesTransform, force: true, ghostChance: ghostChance);
        }

        private void InitStrategy()
        {
            _strategy = _playMode switch
            {
                NeighborsStrategy.MAZE => new MazeStrategy(_minDistanceFromBoss, _maxDistanceFromBoss,
                    _totalNumberOfRooms, _fountainCount, _treasureCount),
                NeighborsStrategy.ENDLESS => new EndlessStrategy(),
                NeighborsStrategy.TUTORIAL => new TutorialStrategy(TutorialLength),
                NeighborsStrategy.BOSS => new BossStrategy(),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (_playMode is NeighborsStrategy.MAZE or NeighborsStrategy.TUTORIAL)
            {
                var index = _playMode is NeighborsStrategy.MAZE
                    ? ((MazeStrategy) _strategy).GetBossRoom()
                    : Vector2Int.up * TutorialLength;
                _bossRoomParticlesForceField = Instantiate(_bossRoomForceFieldPrefab,
                    GetPosition(index), Quaternion.identity);
                _bossRoomParticles = Instantiate(_bossRoomParticlesPrefab);
            }
        }


        private static void InitContentInNeighbors()
        {
            _instance.InitContentInNeighbors_Inner();
        }

        private void InitContentInNeighbors_Inner()
        {
            foreach (Direction dir in DirectionExt.GetDirections())
            {
                var neighborNode = _currentRoom[dir];
                if (neighborNode == null) continue;

                FillRoom(neighborNode);
            }
        }

        private void FillRoom(RoomNode node)
        {
            RoomType type = _strategy.RoomType(node.Index);
            switch (type)
            {
                case RoomType.MONSTERS when !node.Cleared:
                    node.ChooseEnemies();
                    SpawnEnemies(node);
                    node.Interacted = true;
                    break;
                case RoomType.FOUNTAIN:
                case RoomType.TREASURE:
                    node.Room.InitInteractable(type);
                    node.Cleared = true;
                    break;
                case RoomType.TUTORIAL:
                    node.Cleared = true;
                    FillTutorialRoom(node);
                    break;
            }
        }

        private void FinishedCurrentRoom()
        {
            _currentRoom.Cleared = true;
            _currentRoom.Room.GateClosing = false;
            InitContentInNeighbors_Inner();
        }

        #endregion

        #region Types

        private class DoubleRoomManagerException : Exception
        {
        }

        public enum NeighborsStrategy : byte
        {
            MAZE,
            ENDLESS,
            BOSS,
            TUTORIAL
        }

        [Serializable]
        public struct InteractablePair
        {
            public RoomType RoomType;
            public Interactable Interactable;
        }

        #endregion
    }
}