using UnityEngine;

namespace Enemies
{
    public class PrepareAttackBehaviour<T> : EnemyBehaviour<T> where T : Enemy
    {
        private static readonly int PrepareSpeed = Animator.StringToHash("Prepare Speed");
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            
            ThisEnemy.Stop();
            
            ThisEnemy.Attacking = true;
            SetSpeedMultiplier(animator, stateInfo, PrepareSpeed, ThisEnemy.PrepareAttackTime);
            
            ThisEnemy.DesiredDirection = Player.position - ThisEnemy.transform.position;
        }
    }
}