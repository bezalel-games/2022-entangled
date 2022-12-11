using System.Collections;
using Player;
using UnityEngine;

namespace Cards.Buffs.PassiveBuffs
{
    public class EnlargeYoyo : Buff
    {
        [SerializeField] private float _enlargementSpeed = 1;
        private readonly float _growthIncrease;

        public EnlargeYoyo(float growthIncrease)
        {
            _growthIncrease = growthIncrease;
        }

        protected override void ApplyBuff(PlayerController playerController)
        {
            var yoyo = playerController.Yoyo;
            yoyo.StartCoroutine(Enlarge(yoyo.transform));
        }

        private IEnumerator Enlarge(Transform transform)
        {
            var initialScale = transform.localScale;
            var startTime = Time.time;
            while (true)
            {
                yield return null;
                var growthFactor = (1 + Time.time - startTime) * _enlargementSpeed;
                if (growthFactor > _growthIncrease)
                {
                    transform.localScale = initialScale * _growthIncrease;
                    break;
                }
                transform.localScale = initialScale * growthFactor;
            }
        }
    }
}