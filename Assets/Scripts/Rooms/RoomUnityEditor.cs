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
            var camPosition = properties.Offset;
            camPosition.y += properties.KeepInCenter ? 0 : properties.OrthoSize - properties.Height/2f;
            cam.transform.position = camPosition;
        }

        private void SetCollider(RoomProperties.Calculations properties)
        {
            var collider = GetComponent<BoxCollider2D>();
            collider.size = new Vector2(properties.Width, properties.Height);
            var offset = properties.Offset;
            offset.y -= properties.KeepInCenter ? 0 : properties.OrthoSize - properties.Height/2f;
            collider.offset = offset;
        }

        private void SetTileMap(RoomProperties.Calculations properties)
        {
            Tilemap back = RoomContent.transform.Find("Background Tilemap").GetComponent<Tilemap>();
            Tilemap front = RoomContent.transform.Find("Foreground Tilemap").GetComponent<Tilemap>();
                
            back.ClearAllTiles();
            front.ClearAllTiles();
            var pos = Vector3Int.zero;
            for (int x = properties.Left; x <= properties.Right; x++)
                for (int y = properties.Bottom; y <= properties.Top; y++)
                {
                    pos.x = x;
                    pos.y = y;
                    if (x - properties.Left < properties.WallSize || properties.Right - x < properties.WallSize ||
                        y - properties.Bottom < properties.WallSize || properties.Top - y < properties.WallSize)
                    {
                        if (x >= properties.GateLeft && x <= properties.GateRight)
                        {
                            back.SetTile(pos, properties.GateTile);
                            front.SetTile(pos, properties.GateFrameTile);
                            front.SetTile(pos + Vector3Int.down * Math.Sign(y), properties.EmptyTile);
                            if (front.GetTile(pos + Vector3Int.left) == null)
                                front.SetTile(pos + Vector3Int.left, properties.EmptyTile);
                            if (front.GetTile(pos + Vector3Int.right) == null)
                                front.SetTile(pos + Vector3Int.right, properties.EmptyTile);
                            continue;
                        }

                        if (y >= properties.GateBottom && y <= properties.GateTop)
                        {
                            back.SetTile(pos, properties.GateTile);
                            front.SetTile(pos, properties.GateFrameTile);
                            front.SetTile(pos + Vector3Int.left * Math.Sign(x), properties.EmptyTile);
                            if (front.GetTile(pos + Vector3Int.up) == null)
                                front.SetTile(pos + Vector3Int.up, properties.EmptyTile);
                            if (front.GetTile(pos + Vector3Int.down) == null)
                                front.SetTile(pos + Vector3Int.down, properties.EmptyTile);
                            continue;
                        }

                        back.SetTile(pos, properties.WallTile);
                        continue;
                    }

                    back.SetTile(pos, properties.GroundTile);
                }
        }

        #endregion
    }
}
#endif