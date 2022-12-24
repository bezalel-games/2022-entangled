using System.Collections;
using Cards.CardElementClasses;
using Player;
using UnityEngine;

namespace Cards.Buffs.PassiveBuffs
{
    public class EnlargeYoyo : Buff
    {
        #region Fields

        private const float ENLARGEMENT_SPEED = 1;
        private readonly float _growthIncrease;

        #endregion

        #region Buff Implementation

        public override void Apply(PlayerController playerController)
        {
            var yoyo = playerController.Yoyo;
            yoyo.StartCoroutine(Enlarge(yoyo));
        }

        #endregion

        #region Constructor

        public EnlargeYoyo(CardElementClassAttributes attributes, Rarity rarity, float growthIncrease)
            : base(attributes, rarity)
        {
            _growthIncrease = growthIncrease;
        }

        #endregion

        #region Private Methods

        private IEnumerator Enlarge(Yoyo yoyo)
        {
            var initialScale = yoyo.Size;
            var startTime = Time.time;
            while (true)
            {
                yield return null;
                var growthFactor = 1 + (Time.time - startTime) * ENLARGEMENT_SPEED;
                if (growthFactor > _growthIncrease)
                {
                    yoyo.Size = initialScale * _growthIncrease;
                    break;
                }

                yoyo.Size = initialScale * growthFactor;
            }
        }

        #endregion
    }
}