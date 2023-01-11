using System;
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

        public void ChangeStructure(RoomProperties.Calculations calculations, RoomProperties properties)
        {
            RoomProperties = properties;
            SetCam(calculations);
            SetCollider(calculations);
            SetTileMap(calculations);

            if (Application.isPlaying)
                RoomManager.RepositionRoom(this);
        }

        #endregion

        #region Private Methods

        private void SetCam(RoomProperties.Calculations calculations)
        {
            var cam = GetComponentInChildren(typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
            cam.m_Lens.OrthographicSize = calculations.OrthoSize;
            var camPosition = calculations.Offset;
            camPosition.y += calculations.KeepInCenter ? 0 : calculations.OrthoSize - calculations.Height / 2f;
            camPosition.z = -100;
            cam.transform.position = camPosition;
        }

        private void SetCollider(RoomProperties.Calculations calculations)
        {
            var collider = GetComponent<BoxCollider2D>();
            collider.size = new Vector2(calculations.Width, calculations.Height);
            var offset = calculations.Offset;
            offset.y -= calculations.KeepInCenter ? 0 : calculations.OrthoSize - calculations.Height / 2f;
            collider.offset = offset;
        }

        private void SetTileMap(RoomProperties.Calculations calculations)
        {
            Tilemap back = RoomContent.transform.Find("Background Tilemap").GetComponent<Tilemap>();
            Tilemap front = RoomContent.transform.Find("Foreground Tilemap").GetComponent<Tilemap>();

            back.ClearAllTiles();
            front.ClearAllTiles();
            var pos = Vector3Int.zero;
            for (int x = calculations.Left; x <= calculations.Right; x++)
                for (int y = calculations.Bottom; y <= calculations.Top; y++)
                {
                    pos.x = x;
                    pos.y = y;
                    var topOrBottomWall = y - calculations.Bottom < calculations.WallSize ||
                              calculations.Top - y < calculations.WallSize;
                    var leftOrRightWall = x - calculations.Left < calculations.WallSize ||
                              calculations.Right - x < calculations.WallSize;
                    if (topOrBottomWall && x >= calculations.GateLeft && x <= calculations.GateRight)
                    {
                        back.SetTile(pos, calculations.GateTile);
                        front.SetTile(pos, calculations.GateFrameTile);
                        front.SetTile(pos + Vector3Int.down * Math.Sign(y), calculations.EmptyTile);
                        if (front.GetTile(pos + Vector3Int.left) == null)
                            front.SetTile(pos + Vector3Int.left, calculations.EmptyTile);
                        if (front.GetTile(pos + Vector3Int.right) == null)
                            front.SetTile(pos + Vector3Int.right, calculations.EmptyTile);
                        continue;
                    }

                    if (leftOrRightWall && y >= calculations.GateBottom && y <= calculations.GateTop)
                    {
                        back.SetTile(pos, calculations.GateTile);
                        front.SetTile(pos, calculations.GateFrameTile);
                        front.SetTile(pos + Vector3Int.left * Math.Sign(x), calculations.EmptyTile);
                        if (front.GetTile(pos + Vector3Int.up) == null)
                            front.SetTile(pos + Vector3Int.up, calculations.EmptyTile);
                        if (front.GetTile(pos + Vector3Int.down) == null)
                            front.SetTile(pos + Vector3Int.down, calculations.EmptyTile);
                        continue;
                    }
                    if (topOrBottomWall || leftOrRightWall)
                    {

                        back.SetTile(pos, calculations.WallTile);
                        continue;
                    }

                    back.SetTile(pos, calculations.GroundTile);
                }
        }

        #endregion
    }
}
#endif