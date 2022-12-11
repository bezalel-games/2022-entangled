using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tiles
{
    [CreateAssetMenu(fileName = "Extended Rule Tile", menuName = "2D/Tiles/Extended Rule Tile", order = 2)]
    public class ExtendedRuleTile : RuleTile
    {
        public override Type m_NeighborType => typeof(Neighbor);
        private enum Neighbor
        {
            NotEmpty = 0,
            This = 1,
            NotThis = 2
        }

        public override bool RuleMatch(int neighbor, TileBase other)
        {
            if (other is RuleOverrideTile ot)
                other = ot.m_InstanceTile;

            switch ((Neighbor)neighbor)
            {
                case Neighbor.NotEmpty: return other != null;
                case Neighbor.This: return other == this;
                case Neighbor.NotThis: return other != this;
            }

            return true;
        }
    }
}