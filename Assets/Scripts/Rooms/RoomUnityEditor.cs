using Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
namespace Rooms
{
    public partial class Room
    {
        #region Function Events

        private void Awake() => RoomProperties.Changed += ChangeStructure;

        private void OnDestroy() => RoomProperties.Changed -= ChangeStructure;

        #endregion

        #region Public Methods

        public void ChangeStructure(RoomProperties.Calculations properties)
        {
            SetCam(properties);
            SetCollider(properties);
            SetTileMap(properties);

            if (Application.isPlaying)
                RoomManager.RepositionRoom(this);
        }

        #endregion

        #region Private Methods

        private void SetCam(RoomProperties.Calculations properties)
        {
            var cam = GetComponentInChildren(typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
            cam.m_Lens.OrthographicSize = properties.OrthoSize;
            var camPosition = cam.transform.position;
            camPosition.y = properties.Offset;
            cam.transform.position = camPosition;
        }

        private void SetCollider(RoomProperties.Calculations properties)
        {
            var collider = GetComponent<BoxCollider2D>();
            collider.size = new Vector2(properties.Width, properties.Height);
            collider.offset = new Vector2(0, properties.Offset);
        }

        private void SetTileMap(RoomProperties.Calculations properties)
        {
            var tilemap = GetComponentInChildren(typeof(Tilemap), true) as Tilemap;
            tilemap.ClearAllTiles();
            var pos = Vector3Int.zero;
            for (int x = properties.Left; x <= properties.Right; x++)
                for (int y = properties.Bottom; y <= properties.Top; y++)
                {
                    pos.x = x;
                    pos.y = y;
                    if (x - properties.Left < properties.WallSize || properties.Right - x < properties.WallSize ||
                        y - properties.Bottom < properties.WallSize || properties.Top - y < properties.WallSize)
                    {
                        if (x >= properties.GateLeft && x <= properties.GateRight ||
                            y >= properties.GateBottom && y <= properties.GateTop)
                        {
                            tilemap.SetTile(pos, properties.GateTile);
                            continue;
                        }
                        tilemap.SetTile(pos, properties.WallTile);
                        continue;
                    }
                    tilemap.SetTile(pos, properties.GroundTile);
                }
        }

        #endregion
    }
}
#endif