using Audio;
using UnityEngine;

namespace Enemies
{
    public class GoombaPrepareAttack : PrepareAttackBehaviour<Goomba>
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            ThisEnemy.PreparingAttack = true;

            ((IAudible<EnemySounds>) ThisEnemy).PlayOneShot(EnemySounds.GOOMBA_PREP);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            ThisEnemy.PreparingAttack = false;
        }
    }
}