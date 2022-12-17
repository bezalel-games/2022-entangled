using UnityEngine;

namespace Enemies.Goomba
{
    public class GoombaPrepareAttack : GoombaBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            SetSpeedMultiplier(animator, stateInfo, "Prepare Speed", ThisEnemy.PrepareAttackTime);
        }
    }
}