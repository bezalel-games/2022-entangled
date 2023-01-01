using Managers;
using UnityEngine;

namespace Enemies
{
    public abstract class EnemyBehaviour<T> : StateMachineBehaviour where T : Enemy
    {

        #region Fields

        private bool _init;
        protected Transform Player;
        protected T ThisEnemy;

        private readonly int _frameRate = 100;
        
        private int _frameCount;
        private int _frameOffset; // add offset to frame count so enemies will make decisions at different times

        #endregion

        #region Property

        protected bool AtFrameRate => (_frameCount + _frameOffset) % _frameRate == 0;

        #endregion

        #region StateMachineBehaviour

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!_init)
            {
                Init(animator);
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            _frameCount++;
        }

        #endregion

        #region Private Methods

        private void Init(Animator animator)
        {
            _frameOffset = Random.Range(0, 100);
            
            Player = GameManager.PlayerTransform;
            
            ThisEnemy = animator.GetComponent<T>() ?? animator.GetComponentInParent<T>();

            _init = true;
        }
        
        protected void SetSpeedMultiplier(Animator animator, AnimatorStateInfo stateInfo, int parameterHash, float wantedTime)
        {
            var mult = animator.GetFloat(parameterHash);
            var animationLength = stateInfo.length;

            if(animationLength == 0) return;

            // orig * mult = 1/wanted -> mult = wanted * orig
            var originalLength = animationLength * mult;
            var newMult = 1 / (wantedTime * originalLength);
            animator.SetFloat(parameterHash, newMult);
        }

        #endregion
    }
}