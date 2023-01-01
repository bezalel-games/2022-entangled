using UnityEngine;

namespace Enemies
{
    public class GoombaIdle : IdleBehaviour<Goomba>
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(Move, true);
        }
    }
}