using UnityEngine;

namespace Enemies
{
    public abstract class EnemyBehaviour<T> : StateMachineBehaviour
    {
        private bool _init;
        protected Transform Player;
        protected T ThisEnemy;
    
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!_init)
            {
                Init(animator);
            }
        }
    
        private void Init(Animator animator)
        {
            Player = GameManager.PlayerTransform;
            
            ThisEnemy = animator.GetComponent<T>() ?? animator.GetComponentInParent<T>();

            _init = true;
        }
        
        protected void SetSpeedMultiplier(Animator animator, AnimatorStateInfo stateInfo, string parameterName, float wantedTime)
        {
            var mult = animator.GetFloat(parameterName);
            var animationLength = stateInfo.length;

            if(animationLength == 0) return;
        
            var newMult = wantedTime / (animationLength * mult);
            animator.SetFloat(parameterName, newMult);
        }
    }
}