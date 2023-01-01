using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Enemies
{
    public abstract class EnemyBehaviour<T> : StateMachineBehaviour where T : Enemy
    {

        #region Fields

        private bool _init;
        protected Transform Player;
        protected T ThisEnemy;

        private readonly int frameRate = 100;
        
        private int frameCount;
        private int frameOffset; // add offset to frame count so enemies will make decisions at different times

        #endregion

        #region Property

        protected bool AtFrameRate => (frameCount + frameOffset) % frameRate == 0;

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
            frameCount++;
        }

        #endregion

        #region Private Methods

        private void Init(Animator animator)
        {
            frameOffset = UnityEngine.Random.Range(0, 100);
            
            Player = GameManager.PlayerTransform;
            
            ThisEnemy = animator.GetComponent<T>() ?? animator.GetComponentInParent<T>();

            _init = true;
        }
        
        protected void SetSpeedMultiplier(Animator animator, AnimatorStateInfo stateInfo, string parameterName, float wantedTime)
        {
            var mult = animator.GetFloat(parameterName);
            var animationLength = stateInfo.length;

            if(animationLength == 0) return;

            // orig * mult = 1/wanted -> mult = wanted * orig
            var originalLength = animationLength * mult;
            var newMult = 1 / (wantedTime * originalLength);
            animator.SetFloat(parameterName, newMult);
        }

        #endregion
    }
}