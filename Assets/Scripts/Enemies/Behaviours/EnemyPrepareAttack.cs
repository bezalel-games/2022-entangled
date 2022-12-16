using Unity.VisualScripting;
using UnityEngine;

namespace Enemies
{
    public class EnemyPrepareAttack : EnemyBehaviour<Enemy>
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            SetSpeedMultiplier(animator, stateInfo, "Prepare Attack", ThisEnemy.PrepareAttackTime);
        }
    }
}