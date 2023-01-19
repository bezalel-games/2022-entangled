using System;
using System.Linq;
using Cards.Buffs;
using Cards.Buffs.ActiveBuffs;
using Cards.Buffs.Components;
using Cards.Buffs.PassiveBuffs;
using Cards.CardElementClasses;
using Cards.Debuffs;
using UnityEngine;
using static Cards.Factory.BuffType;
using static Cards.Factory.DebuffType;

namespace Cards.Factory
{
    [CreateAssetMenu(fileName = "Card Factory", menuName = "Entangled/Cards/Card Factory", order = 0)]
    public class CardFactory : ScriptableObject
    {
        #region Buff Serialized Fields

        [field: Header("Buffs")]
        [field: Tooltip("Multiply the current size of the yoyo by given amount")]
        [field: SerializeField] public VariableCardElementClass EnlargeYoyo { get; private set; }
        
        [field: Tooltip("Leave the trail of the precision shot on the map, dealing damage on touch")]
        [field: SerializeField] public FixedCardElementClass LeaveTrail { get; private set; }
        
        [field: Tooltip("Create an explosion with the given radius around the yoyo")]
        [field: SerializeField] public FixedCardElementClass ExplosiveYoyo { get; private set; }

        [field: SerializeField] public Explosion ExplosionPrefab { get; private set; }

        [field: Tooltip("Teleports to the yoyo's location, using the specified amount of stamina")]
        [field: SerializeField] public FixedCardElementClass SwapPositionWithYoyo { get; private set; }

        [field: Tooltip("Multiply the current yoyo explosion radius by given amount")]
        [field: SerializeField] public VariableCardElementClass ExpandExplosion { get; private set; }

        [field: Tooltip("Multiply the current yoyo explosion damage by given amount")]
        [field: SerializeField] public VariableCardElementClass IncreaseExplosionDamage { get; private set; }
        
        [field: Tooltip("Multiply the current trail stay time by given amount")]
        [field: SerializeField] public VariableCardElementClass IncreaseTrailStay { get; private set; }
        
        [field: Tooltip("Multiply the current trail damage by given amount")]
        [field: SerializeField] public VariableCardElementClass IncreaseTrailDamage { get; private set; }
        #endregion

        #region Debuff Serialized Fields

        [field: Header("Debuffs")]
        [field: Tooltip("Multiplies the number of Goombas by the specified amount")]
        [field: SerializeField] public EnemyVariableCardElementClass MoreGoombas { get; private set; }

        [field: Tooltip("Multiplies the number of Shooters by the specified amount")]
        [field: SerializeField] public EnemyVariableCardElementClass MoreShooters { get; private set; }

        [field: Tooltip("Multiplies the number of Fumers by the specified amount")]
        [field: SerializeField] public EnemyVariableCardElementClass MoreFumers { get; private set; }

        [field: Tooltip("Multiplies Goomba HP by the specified amount")]
        [field: SerializeField] public EnemyVariableCardElementClass TougherGoombas { get; private set; }

        [field: Tooltip("Multiplies Shooter HP by the specified amount")]
        [field: SerializeField] public EnemyVariableCardElementClass TougherShooters { get; private set; }

        [field: Tooltip("Multiplies Fumer HP by the specified amount")]
        [field: SerializeField] public EnemyVariableCardElementClass TougherFumers { get; private set; }

        [field: Tooltip("Multiplies Goomba speed by the specified amount")]
        [field: SerializeField] public EnemyVariableCardElementClass FasterGoombas { get; private set; }

        [field: Tooltip("Multiplies Shooter speed by the specified amount")]
        [field: SerializeField] public EnemyVariableCardElementClass FasterShooters { get; private set; }

        [field: Tooltip("Multiplies Fumer speed by the specified amount")]
        [field: SerializeField] public EnemyVariableCardElementClass FasterFumers { get; private set; }
        
        [field: Tooltip("Multiplies Quickshot's damage by the specified amount")]
        [field: SerializeField] public VariableCardElementClass DecreaseDamage { get; private set; }
        
        [field: Tooltip("Multiplies Quickshot's distance by the specified amount")]
        [field: SerializeField] public VariableCardElementClass DecreaseShotDistance { get; private set; }
        
        [field: Tooltip("Multiplies MP regeneration per second by the specified amount")]
        [field: SerializeField] public VariableCardElementClass DecreaseMpRegen { get; private set; }
        
        [field: Tooltip("Shooters' projectiles follow the player")]
        [field: SerializeField] public EnemyFixedCardElementClass HomingProjectiles { get; private set; }
        
        [field: Tooltip("Goombas split to two on death")]
        [field: SerializeField] public EnemyFixedCardElementClass SplittingGoombas { get; private set; }
        
        [field: Tooltip("Shooters split to two on death")]
        [field: SerializeField] public EnemyFixedCardElementClass SplittingShooters { get; private set; }
        
        [field: Tooltip("Fumers split to two on death")]
        [field: SerializeField] public EnemyFixedCardElementClass SplittingFumers { get; private set; }
        
        #endregion

        #region Public Methods

        public Card Create(BuffType buffType, Rarity buffRarity, DebuffType debuffType, Rarity debuffRarity)
        {
            Buff buff = buffType switch
            {
                ENLARGE_YOYO => new EnlargeYoyo(EnlargeYoyo.Attributes, buffRarity, EnlargeYoyo[buffRarity]),
                EXPLOSIVE_YOYO => new ExplosiveYoyo(ExplosiveYoyo.Attributes, buffRarity, ExplosiveYoyo.Parameters[0],
                    ExplosionPrefab),
                SWAP_POSITIONS_WITH_YOYO => new SwapPositionWithYoyo(SwapPositionWithYoyo.Attributes, buffRarity,
                    SwapPositionWithYoyo.Parameters[0]),
                EXPAND_EXPLOSION => new ExpandExplosion(ExpandExplosion.Attributes, buffRarity,
                    ExpandExplosion[buffRarity]),
                INCREASE_EXPLOSION_DAMAGE => new IncreaseExplosionDamage(IncreaseExplosionDamage.Attributes, buffRarity,
                    IncreaseExplosionDamage[buffRarity]),
                LEAVE_TRAIL => new LeaveTrail(LeaveTrail.Attributes, buffRarity, 
                    LeaveTrail.Parameters[0], LeaveTrail.Parameters[1]),
                INCREASE_TRAIL_STAY => new IncreaseTrailStay(IncreaseTrailStay.Attributes, buffRarity, 
                    IncreaseTrailStay[buffRarity]),
                INCREASE_TRAIL_DAMAGE => new IncreaseTrailDamage(IncreaseTrailDamage.Attributes, buffRarity, 
                    IncreaseTrailDamage[buffRarity]),
                _ => throw new ArgumentOutOfRangeException(nameof(buffType), buffType, null)
            };
            Debuff debuff = debuffType switch
            {
                MORE_GOOMBAS => new MoreEnemies(MoreGoombas.Attributes, debuffRarity, MoreGoombas.EnemyIndex,
                    MoreGoombas[debuffRarity]),
                MORE_SHOOTERS => new MoreEnemies(MoreShooters.Attributes, debuffRarity, MoreShooters.EnemyIndex,
                    MoreShooters[debuffRarity]),
                MORE_FUMERS => new MoreEnemies(MoreFumers.Attributes, debuffRarity, MoreFumers.EnemyIndex,
                    MoreFumers[debuffRarity]),
                TOUGHER_GOOMBAS => new TougherEnemies(TougherGoombas.Attributes, debuffRarity,
                    TougherGoombas.EnemyIndex,
                    TougherGoombas[debuffRarity]),
                TOUGHER_SHOOTERS => new TougherEnemies(TougherShooters.Attributes, debuffRarity,
                    TougherShooters.EnemyIndex,
                    TougherShooters[debuffRarity]),
                TOUGHER_FUMERS => new TougherEnemies(TougherFumers.Attributes, debuffRarity,
                    TougherFumers.EnemyIndex,
                    TougherFumers[debuffRarity]),
                FASTER_GOOMBAS => new FasterEnemies(FasterGoombas.Attributes, debuffRarity,
                    FasterGoombas.EnemyIndex,
                    FasterGoombas[debuffRarity]),
                FASTER_SHOOTERS => new FasterEnemies(FasterShooters.Attributes, debuffRarity,
                    FasterShooters.EnemyIndex,
                    FasterShooters[debuffRarity]),
                FASTER_FUMERS => new FasterEnemies(FasterFumers.Attributes, debuffRarity,
                    FasterFumers.EnemyIndex,
                    FasterFumers[debuffRarity]),
                DECREASE_DAMAGE => new DecreaseDamage(DecreaseDamage.Attributes, debuffRarity, 
                    DecreaseDamage[debuffRarity]),
                DECREASE_SHOT_DISTANCE => new DecreaseShotDistance(DecreaseShotDistance.Attributes, debuffRarity, 
                    DecreaseShotDistance[debuffRarity]),
                DECREASE_MP_REGEN => new DecreaseMpRegeneration(DecreaseMpRegen.Attributes, debuffRarity, 
                    DecreaseMpRegen[debuffRarity]),
                HOMING_SHOTS => new HomingShots(HomingProjectiles.Attributes, debuffRarity, 
                    HomingProjectiles.EnemyIndex),
                SPLIT_GOOMBAS => new SplitOnDeath(SplittingGoombas.Attributes, debuffRarity,
                    SplittingGoombas.EnemyIndex, (int)SplittingGoombas.Parameters[0]),
                SPLIT_SHOOTERS => new SplitOnDeath(SplittingShooters.Attributes, debuffRarity,
                    SplittingShooters.EnemyIndex, (int)SplittingShooters.Parameters[0]),
                SPLIT_FUMERS => new SplitOnDeath(SplittingFumers.Attributes, debuffRarity,
                    SplittingFumers.EnemyIndex, (int)SplittingFumers.Parameters[0]),
                _ => throw new ArgumentOutOfRangeException(nameof(debuffType), debuffType, null)
            };
            return new Card(buff, debuff);
        }

        #endregion
    }
}