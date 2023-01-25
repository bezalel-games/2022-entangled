using System;
using Rooms;
using Unity.VisualScripting.FullSerializer;
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

        public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap,
            ref TileAnimationData tileAnimationData)
        {
            var result = base.GetTileAnimationData(position, tilemap, ref tileAnimationData);
            if (result)
                tileAnimationData.animationStartTime = Time.time;
            return result;
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            var iden = Matrix4x4.identity;
            
            Vector3Int approxActualTilePosition = ((Vector3Int)RoomManager.CurrentRoomIndex * 16) + position;
            
            tileData.sprite = m_DefaultSprite;
            tileData.gameObject = m_DefaultGameObject;
            tileData.colliderType = m_DefaultColliderType;
            tileData.flags = TileFlags.LockTransform;
            tileData.transform = iden;

            Matrix4x4 transform = iden;
            foreach (TilingRule rule in m_TilingRules)
            {
                if (RuleMatches(rule, position, tilemap, ref transform))
                {
                    switch (rule.m_Output)
                    {
                        case TilingRuleOutput.OutputSprite.Single:
                        case TilingRuleOutput.OutputSprite.Animation:
                            tileData.sprite = rule.m_Sprites[0];
                            break;
                        case TilingRuleOutput.OutputSprite.Random:
                            int index = Mathf.Clamp(
                                Mathf.FloorToInt(
                                    GetPerlinValue(approxActualTilePosition,
                                        rule.m_PerlinScale, 100000f) * rule.m_Sprites.Length), 0,
                                rule.m_Sprites.Length - 1);
                            tileData.sprite = rule.m_Sprites[index];
                            if (rule.m_RandomTransform != TilingRuleOutput.Transform.Fixed)
                                transform = ApplyRandomTransform(rule.m_RandomTransform, transform, rule.m_PerlinScale,
                                    position);
                            break;
                    }

                    tileData.transform = transform;
                    tileData.gameObject = rule.m_GameObject;
                    tileData.colliderType = rule.m_ColliderType;
                    break;
                }
            }
        }
    }
}