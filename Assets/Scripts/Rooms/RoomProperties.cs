using UnityEngine;
using UnityEngine.Tilemaps;

namespace Rooms
{
    [CreateAssetMenu(fileName = "Room Properties Asset", menuName = "Room Properties Asset", order = 0)]
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
        [field: SerializeField] public TileBase GateTile { get; private set; }

        [Space(20)] [SerializeField] private bool _update;
        [Space(20)] [SerializeField] private bool _updateContinuously;

        #endregion
    }
}