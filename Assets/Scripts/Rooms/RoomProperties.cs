using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Rooms
{
    [CreateAssetMenu(fileName = "Room Properties Asset", menuName = "Entangled/Room Properties Asset", order = 0)]
    public partial class RoomProperties : ScriptableObject
    {
        #region Serialized Fields

        [field: SerializeField] private Room Prefab { get; set; }

        [Space(20)]
        [SerializeField] private bool _useRatio;

        [field: Header("If using ratio:")]
        [field: Range(1, 12)] [SerializeField] private int _ratioMultiplicationFactor = 1;

        [SerializeField] private Vector2Int _ratio = new(16, 9);

        [field: Header("If not using ratio:")]
        [field: Range(4, 192)]
        [field: SerializeField] public int Width { get; private set; } = 16;

        [field: Range(4, 108)]
        [field: SerializeField] public int Height { get; private set; } = 9;

        [field: Space(20)]
        [field: SerializeField] public int VerticalIntersection { get; private set; } = 1;

        [field: SerializeField] public int HorizontalIntersection { get; private set; } = 1;
        [field: SerializeField] public int WallSize { get; private set; } = 1;
        [field: SerializeField] public int GateWidth { get; private set; } = 2;

        [field: Header("Tiles")]
        [field: SerializeField] public TileBase GroundTile { get; private set; }

        [field: SerializeField] public TileBase WallTile { get; private set; }
        [field: SerializeField] public TileBase ClosedGateTile { get; private set; }
        [field: SerializeField] public TileBase OpenedGateTile { get; private set; }

        [Space(20)] [SerializeField] private bool _update;
        [Space(20)] [SerializeField] private bool _updateContinuously;

        #endregion

        #region Non-Serialized Fields

        private List<Vector3Int> _gatePositions = new();

        #endregion

        #region Properties

        public IEnumerable<Vector3Int> GatePositions
        {
            get
            {
                if (_gatePositions.Count == 0)
                    CalculateGatePositions();
                return _gatePositions;
            }
        }

        #endregion

        private void CalculateGatePositions()
        {
            var halfHeight = (Height - 1) / 2f;
            var halfWidth = (Width - 1) / 2f;
            var top = Mathf.CeilToInt(halfHeight);
            var bottom = -Mathf.FloorToInt(halfHeight);
            var right = Mathf.FloorToInt(halfWidth);
            var left = -Mathf.CeilToInt(halfWidth);
            var halfGateWidth = (GateWidth - 1) / 2f;
            var gateTop = Mathf.CeilToInt(halfGateWidth);
            var gateBottom = -Mathf.FloorToInt(halfGateWidth);
            var gateRight = Mathf.FloorToInt(halfGateWidth);
            var gateLeft = -Mathf.CeilToInt(halfGateWidth);
            var pos = Vector3Int.zero;
            for (int x = left; x <= right; x++)
                for (int y = bottom; y <= top; y++)
                {
                    pos.x = x;
                    pos.y = y;
                    if (x - left >= WallSize && right - x >= WallSize && y - bottom >= WallSize &&
                        top - y >= WallSize) continue;
                    if (x >= gateLeft && x <= gateRight || y >= gateBottom && y <= gateTop)
                        _gatePositions.Add(pos);
                }
        }
    }
}