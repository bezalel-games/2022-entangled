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
            yoyo.StartCoroutine(Enlarge(yoyo.GetComponentInChildren<Collider2D>().transform));
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

        private IEnumerator Enlarge(Transform transform)
        {
            var initialScale = transform.localScale;
            var startTime = Time.time;
            while (true)
            {
                yield return null;
                var growthFactor = 1 + (Time.time - startTime) * ENLARGEMENT_SPEED;
                if (growthFactor > _growthIncrease)
                {
                    transform.localScale = initialScale * _growthIncrease;
                    break;
                }

                transform.localScale = initialScale * growthFactor;
            }
        }

        #endregion
    }
}