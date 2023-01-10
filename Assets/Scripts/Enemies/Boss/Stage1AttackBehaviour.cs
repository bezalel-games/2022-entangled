using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies.Boss
{
    public class Stage1AttackBehaviour : BossBehaviour
    {
        #region Serialized Fields

        #endregion

        #region Non-Serialized Fields

        private delegate IEnumerator<int?> ThrowOrder(int count);

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

        private IEnumerator<int?> ThreeSets(int count)
        {
            Debug.Log("Three sets");
            for (int set = 0; set < 3; ++set)
            {
                for (int i = set; i < count; i += 3)
                    yield return (i + _nextThrowIndex) % count;
                yield return null;
            }
            
        }

        private IEnumerator<int?> Triples(int count)
        {
            Debug.Log("Triples");
            for (int i = 0; i < count; ++i)
            {
                if (i > 0 && i % 3 == 0)
                    yield return null;
                yield return (i + _nextThrowIndex) % count;
            }
        }

        private IEnumerator<int?> OneAtATime(int count)
        {
            Debug.Log("One at a time");
            for (int i = 0; i < count; ++i)
            {
                yield return (i + _nextThrowIndex) % count;
                yield return null;
            }
        }

        private IEnumerator ThrowYoyos(ThrowOrder throwOrder)
        {
            var enumerator = throwOrder(Boss.YoyoCount);
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