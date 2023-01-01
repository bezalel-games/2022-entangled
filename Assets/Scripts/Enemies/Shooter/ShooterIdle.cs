using UnityEngine;

namespace Enemies
{
    public class ShooterIdle : IdleBehaviour<Shooter>
    {
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(Move, true);
        }
    }
}