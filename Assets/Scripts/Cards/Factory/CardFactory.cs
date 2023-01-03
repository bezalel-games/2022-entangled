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

        [field: Tooltip("Create an explosion with the given radius around the yoyo")]
        [field: SerializeField] public FixedCardElementClass ExplosiveYoyo { get; private set; }

        [field: SerializeField] public Explosion ExplosionPrefab { get; private set; }

        [field: Tooltip("Teleports to the yoyo's location, using the specified amount of stamina")]
        [field: SerializeField] public FixedCardElementClass SwapPositionWithYoyo { get; private set; }

        #endregion

        #region Debuff Serialized Fields

        [field: Header("Debuffs")]
        [field: Tooltip("Multiplies the number of Goombas by the specified amount")]
        [field: SerializeField] public EnemyCardElementClass MoreGoombas { get; private set; }

        [field: Tooltip("Multiplies the number of Shooters by the specified amount")]
        [field: SerializeField] public EnemyCardElementClass MoreShooters { get; private set; }
        
        [field: Tooltip("Multiplies the number of Fumers by the specified amount")]
        [field: SerializeField] public EnemyCardElementClass MoreFumers { get; private set; }
        
        [field: Tooltip("Multiplies Goomba HP by the specified amount")]
        [field: SerializeField] public EnemyCardElementClass TougherGoombas { get; private set; }
        
        [field: Tooltip("Multiplies Shooter HP by the specified amount")]
        [field: SerializeField] public EnemyCardElementClass TougherShooters { get; private set; }
        
        [field: Tooltip("Multiplies Fumer HP by the specified amount")]
        [field: SerializeField] public EnemyCardElementClass TougherFumers { get; private set; }
        
        [field: Tooltip("Multiplies Goomba speed by the specified amount")]
        [field: SerializeField] public EnemyCardElementClass FasterGoombas { get; private set; }

        [field: Tooltip("Multiplies Shooter speed by the specified amount")]
        [field: SerializeField] public EnemyCardElementClass FasterShooters { get; private set; }
        
        [field: Tooltip("Multiplies Fumer speed by the specified amount")]
        [field: SerializeField] public EnemyCardElementClass FasterFumers { get; private set; }

        #endregion

        #region Public Methods

        public Card Create(BuffType buffType, Rarity buffRarity, DebuffType debuffType, Rarity debuffRarity)
        {
            Buff buff = buffType switch
            {
                ENLARGE_YOYO => new EnlargeYoyo(EnlargeYoyo.Attributes, buffRarity, EnlargeYoyo[buffRarity]),
                EXPLOSIVE_YOYO => new ExplosiveYoyo(ExplosiveYoyo.Attributes, buffRarity, ExplosiveYoyo.Parameter, ExplosionPrefab),
                SWAP_POSITIONS_WITH_YOYO => new SwapPositionWithYoyo(SwapPositionWithYoyo.Attributes, buffRarity,
                    SwapPositionWithYoyo.Parameter),
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
                _ => throw new ArgumentOutOfRangeException(nameof(debuffType), debuffType, null)
            };
            return new Card(buff, debuff);
        }

        #endregion
    }
}