using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies.Boss
{
    public class Phase1AttackBehaviour : BossBehaviour
    {
        #region Serialized Fields

        #endregion

        #region Non-Serialized Fields

        private delegate IEnumerator<int?> ThrowOrder(int min, int max);

        private ThrowOrder[] _throws;
        private int _nextThrowIndex = 0;

        #endregion

        #region State Machine Methods

        protected override void OnFirstStateEnter()
        {
            _throws = new ThrowOrder[] { OneAtATime, Triples, ThreeSets, InwardWave, OutwardWave };
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            Boss.StartCoroutine(ThrowYoyos(_throws[_nextThrowIndex++ % _throws.Length]));
        }

        #endregion

        #region Enumerators

        private IEnumerator<int?> ThreeSets(int min, int max)
        {
            for (int set = 0; set < 3; ++set)
            {
                for (int i = set + min; i <= max; i += 3)
                    yield return i;
                yield return null;
            }
        }

        private IEnumerator<int?> Triples(int min, int max)
        {
            int numYoyos = max - min;
            int third = numYoyos / 3;
            int[] setSize = { third, third, third };
            switch (third % 3)
            {
                case 2:
                    ++setSize[0];
                    ++setSize[2];
                    break;
                case 1:
                    ++setSize[1];
                    break;
            }

            int yoyo = min;
            for (int set = 0; set < 3; ++set)
            {
                for (int i = 0; i < setSize[set]; ++i)
                    yield return yoyo++;
                yield return null;
            }
        }

        private IEnumerator<int?> OneAtATime(int min, int max)
        {
            for (int i = min; i <= max; ++i)
            {
                yield return i;
                yield return null;
            }
        }
        
        private IEnumerator<int?> InwardWave(int min, int max)
        {
            int mid = (int) Mathf.Ceil((min + max) / 2f);
            for (int i = min; i < mid; ++i)
            {
                yield return i;
                yield return max - i + min;
                yield return null;
            }
            yield return mid;
        }
        
        private IEnumerator<int?> OutwardWave(int min, int max)
        {
            int mid = (int) Mathf.Ceil((min + max) / 2f);
            yield return mid;
            for (int i = mid - 1; i >= min; --i)
            {
                yield return null;
                yield return i;
                yield return max - i + min;
            }
        }

        private IEnumerator ThrowYoyos(ThrowOrder throwOrder)
        {
            var enumerator = throwOrder(Boss.MinYoyoInRoom, Boss.MaxYoyoInRoom);
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.HasValue)
                {
                    Boss.ShootYoyo(enumerator.Current.Value);
                    continue;
                }

                yield return new WaitForSeconds(Boss.ThrowSetInterval);
            }
        }

        #endregion
    }
}