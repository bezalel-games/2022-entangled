using UnityEngine;

namespace Enemies
{
    public class IdleBehaviour<T> : EnemyBehaviour<T> where T : Enemy
    {
        private static readonly int Move = Animator.StringToHash("Move");

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(Move,true);
        }
    }
}