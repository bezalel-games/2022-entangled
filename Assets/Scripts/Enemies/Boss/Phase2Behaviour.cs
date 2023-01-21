using System.Collections;
using UnityEngine;

namespace Enemies.Boss
{
    public class Phase2Behaviour : BossBehaviour
    {
        #region Non-Serizlized Fields

        private float _t = 0;

        #endregion

        #region State Machine Methods
        
        protected override void OnFirstStateEnter()
        {
            Boss.StartCoroutine(Accelerate());
            Boss.ShieldActive(true);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            Boss.SpinYoyos(_t);
        }
        
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.IsName("Phase 2"))
                Boss.ShieldActive(false);
        }

        #endregion

        private IEnumerator Accelerate()
        {
            float startTime = Time.time;
            float t;
            while ((t = Time.time - startTime) < 1)
            {
                _t = t * t;
                yield return null;
            }

            _t = 1;
        }
    }
}