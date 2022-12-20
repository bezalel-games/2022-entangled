using System;
using Cards.Buffs;
using Cards.Buffs.ActiveBuffs;
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

        [field: SerializeField] public FloatCardElementClass EnlargeYoyo { get; private set; }
        [field: SerializeField] public FloatCardElementClass ExplosiveYoyo { get; private set; }
        [field: SerializeField] public FloatCardElementClass SwapPositionWithYoyo { get; private set; }

        #endregion

        #region Debuff Serialized Fields

        [field: SerializeField] public IntEnemyCardElementClass MoreGoombas { get; private set; }
        [field: SerializeField] public FloatEnemyCardElementClass TougherGoombas { get; private set; }

        [field: SerializeField] public IntEnemyCardElementClass MoreShooters { get; private set; }

        [field: SerializeField] public FloatEnemyCardElementClass TougherShooters { get; private set; }

        #endregion

        #region Function Events

        private void OnValidate()
        {
        }

        #endregion

        #region Public Methods

        public Card GenerateCard(BuffType buffType, Rarity buffRarity, DebuffType debuffType, Rarity debuffRarity)
        {
            Buff buff = buffType switch
            {
                ENLARGE_YOYO => new EnlargeYoyo(EnlargeYoyo.Attributes, buffRarity, EnlargeYoyo[buffRarity]),
                EXPLOSIVE_YOYO => new ExplosiveYoyo(ExplosiveYoyo.Attributes, buffRarity),
                SWAP_POSITIONS_WITH_YOYO => new SwapPositionWithYoyo(SwapPositionWithYoyo.Attributes, buffRarity),
                _ => throw new ArgumentOutOfRangeException(nameof(buffType), buffType, null)
            };
            Debuff debuff = debuffType switch
            {
                MORE_GOOMBAS => new MoreEnemies(MoreGoombas.Attributes, debuffRarity, MoreGoombas.EnemyIndex,
                    MoreGoombas[debuffRarity]),
                TOUGHER_GOOMBAS => new TougherEnemies(TougherGoombas.Attributes, debuffRarity,
                    TougherGoombas.EnemyIndex,
                    TougherGoombas[debuffRarity]),
                MORE_SHOOTERS => new MoreEnemies(MoreShooters.Attributes, debuffRarity, MoreShooters.EnemyIndex,
                    MoreShooters[debuffRarity]),
                TOUGHER_SHOOTERS => new TougherEnemies(TougherShooters.Attributes, debuffRarity,
                    TougherShooters.EnemyIndex,
                    TougherShooters[debuffRarity]),
                _ => throw new ArgumentOutOfRangeException(nameof(debuffType), debuffType, null)
            };
            return new Card(buff, debuff);
        }

        #endregion
    }
}