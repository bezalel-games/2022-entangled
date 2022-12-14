using System.Collections;
using Player;
using UnityEngine;

namespace Cards.Buffs.PassiveBuffs
{
    public class EnlargeYoyo : IBuff
    {
        #region Fields

        private const float ENLARGEMENT_SPEED = 1;
        private readonly float _growthIncrease;

        #endregion

        #region ICardProperty Implementation

        public string Name => "Size Matters";

        public string Description => "Make your weapon bigger";

        public string Rarity => "Normal";

        #endregion

        #region IBuff Implementation

        public void Apply(PlayerController playerController)
        {
            var yoyo = playerController.Yoyo;
            yoyo.StartCoroutine(Enlarge(yoyo.transform));
        }

        #endregion

        #region Constructor

        public EnlargeYoyo(float growthIncrease)
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
                var growthFactor = (1 + Time.time - startTime) * ENLARGEMENT_SPEED;
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