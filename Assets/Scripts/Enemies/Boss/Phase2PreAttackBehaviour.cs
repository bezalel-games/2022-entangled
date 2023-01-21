using UnityEngine;

namespace Enemies.Boss
{
    public class Phase2PreAttackBehaviour : BossBehaviour
    {
        #region Serialized Fields

        #endregion

        #region Non-Serialized Fields

        #endregion

        #region State Machine Methods

        protected override void OnFirstStateEnter()
        {
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            Boss.CreateBomb();
        }

        #endregion
    }
}