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
            _throws = new ThrowOrder[] { OneAtATime, Triples, ThreeSets };
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
            Debug.Log("Three sets");
            for (int set = 0; set < 3; ++set)
            {
                for (int i = set + min; i <= max; i += 3)
                    yield return i;
                yield return null;
            }
        }

        private IEnumerator<int?> Triples(int min, int max)
        {
            Debug.Log("Triples");
            for (int i = min; i <= max; ++i)
            {
                if (i > min && i % 3 == 0)
                    yield return null;
                yield return i;
            }
        }

        private IEnumerator<int?> OneAtATime(int min, int max)
        {
            Debug.Log("One at a time");
            for (int i = min; i <= max; ++i)
            {
                yield return i;
                yield return null;
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