using UnityEngine;

namespace Enemies
{
    public class AttackBehaviour<T> : EnemyBehaviour<T> where T : Enemy
    {
        protected static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int AttackSpeed = Animator.StringToHash("Attack Speed");

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
        
            SetSpeedMultiplier(animator, stateInfo, AttackSpeed, ThisEnemy.AttackTime);
        }
    
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ThisEnemy.Attacking = false;
            
            ThisEnemy.CanAttack = false;
            ThisEnemy.DelayInvoke(() => { ThisEnemy.CanAttack = true;}, ThisEnemy.AttackCooldown);
            
            animator.ResetTrigger(Idle);
        }
    }
}