using UnityEngine;

namespace Enemies.Boss
{
    public abstract class BossBehaviour : StateMachineBehaviour
    {
        private bool _firstEnter = true;
        protected Boss Boss;
        protected Animator Animator;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            if (_firstEnter)
                Init(animator);
        }

        private void Init(Animator animator)
        {
            Animator = animator;
            Boss = animator.GetComponent<Boss>();
            _firstEnter = false;
            OnFirstStateEnter();
        }

        protected virtual void OnFirstStateEnter()
        {
        }
    }
}