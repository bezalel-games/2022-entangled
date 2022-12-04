#if UNITY_EDITOR
using System;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Rooms
{
    public partial class RoomProperties //Unity Editor Extension - only compiles in the editor
    {
        #region Events

        public static event Action<Calculations> Changed;

        #endregion

        #region Function Events

        private void OnValidate()
        {
            if (!_update && !_updateContinuously) return;
            if (_useRatio)
            {
                Width = _ratioMultiplicationFactor * _ratio.x;
                Height = _ratioMultiplicationFactor * _ratio.y;
            }
            else
            {
                Width = Mathf.Max(Width, WallSize + GateWidth);
                Height = Mathf.Max(Height, WallSize + GateWidth);
            }

            VerticalIntersection = Mathf.Min(WallSize, VerticalIntersection);
            HorizontalIntersection = Mathf.Min(WallSize, HorizontalIntersection);
            var size = 1920 / Width; 
            Debug.Log($"Tile size is {size}x{size}");
            _update = false;
            UnityEditor.EditorApplication.delayCall += AfterValidate;
        }

        #endregion

        #region Private Methods

        private void AfterValidate()
        {
            var calculations = new Calculations(this);
            Changed?.Invoke(calculations);
            if (Prefab != null)
                Prefab.ChangeStructure(calculations);
        }

        #endregion

        #region Structs

        public struct Calculations
        {
            private const float WidthToNeededHeight = (float)(9 / 32d);

            public readonly float OrthoSize;
            public readonly float Offset;
            public readonly int Top;
            public readonly int Bottom;
            public readonly int Left;
            public readonly int Right;
            public readonly int Width;
            public readonly int Height;
            public readonly int WallSize;
            public readonly int GateWidth;
            public readonly int GateTop;
            public readonly int GateBottom;
            public readonly int GateLeft;
            public readonly int GateRight;
            public Tile GroundTile { get;}
            public Tile WallTile { get;}
            public Tile GateTile { get;}

            public Calculations(RoomProperties properties)
            {
                Width = properties.Width;
                Height = properties.Height;
                WallSize = properties.WallSize;
                GateWidth = properties.GateWidth;
                
                GroundTile = properties.GroundTile;
                WallTile = properties.WallTile;
                GateTile = properties.GateTile;

                OrthoSize = Mathf.Max(Height / 2f, Width * WidthToNeededHeight);
                Offset = Height % 2 == 0 ? 1 : 0.5f;
                var halfHeight = (Height - 1) / 2f;
                var halfWidth = (Width - 1) / 2f;
                Top = Mathf.CeilToInt(halfHeight);
                Bottom = -Mathf.FloorToInt(halfHeight);
                Right = Mathf.FloorToInt(halfWidth);
                Left = -Mathf.CeilToInt(halfWidth);
                var halfGateWidth = (GateWidth - 1) / 2f;
                GateTop = Mathf.CeilToInt(halfGateWidth);
                GateBottom = -Mathf.FloorToInt(halfGateWidth);
                GateRight = Mathf.FloorToInt(halfGateWidth);
                GateLeft = -Mathf.CeilToInt(halfGateWidth);
            }
        }

        #endregion
    }
}
#endif