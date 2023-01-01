using UnityEngine;

namespace Enemies
{
    public class FumerIdle : IdleBehaviour<Fumer>
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(Move, true);
        }
    }
}