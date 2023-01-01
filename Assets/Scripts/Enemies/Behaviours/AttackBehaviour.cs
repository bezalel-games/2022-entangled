using UnityEngine;

namespace Enemies
{
    public class AttackBehaviour<T> : EnemyBehaviour<T> where T : Enemy
    {
        private static readonly int Idle = Animator.StringToHash("Idle");

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
        
            SetSpeedMultiplier(animator, stateInfo, "Attack Speed", ThisEnemy.AttackTime);
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