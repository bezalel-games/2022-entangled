using UnityEngine;

namespace Enemies
{
    public class PrepareAttackBehaviour<T> : EnemyBehaviour<T> where T : Enemy
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            
            ThisEnemy.Stop();
            
            ThisEnemy.Attacking = true;
            SetSpeedMultiplier(animator, stateInfo, "Prepare Speed", ThisEnemy.PrepareAttackTime);
            
            ThisEnemy.DesiredDirection = Player.position - ThisEnemy.transform.position;
        }
    }
}