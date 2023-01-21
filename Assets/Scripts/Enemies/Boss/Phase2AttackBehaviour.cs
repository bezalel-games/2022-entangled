using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Enemies.Boss
{
    public class Phase2AttackBehaviour : BossBehaviour
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
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            Boss.ThrowBomb(GameManager.PlayerTransform.position);
        }

        #endregion
    }
}