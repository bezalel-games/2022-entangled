using UnityEngine;

namespace Enemies.Shooter
{
    public class ShooterPrepareAttack : ShooterBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            Enemy.NumberOfAttacking++;
            SetSpeedMultiplier(animator, stateInfo, "Prepare Speed", ThisEnemy.PrepareAttackTime);
        }
    }
}