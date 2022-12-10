using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoombaMove : GoombaBehaviour
{

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var playerPos = _player.position;
        var goombaPos = _goomba.transform.position;
        var distance = Vector2.Distance(playerPos, goombaPos);

        // if (distance > _goomba.FollowDistance)
        // {
        //     _goomba.DesiredDirection = Vector2.zero;
        //     animator.SetBool("Move",false);
        // }
        // else
        if (distance <= _goomba.AttackDistance)
        {
            _goomba.DesiredDirection = Vector2.zero;
            animator.SetTrigger("Attack");
        }
        else
        {
            _goomba.DesiredDirection = playerPos - goombaPos;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");
    }
}
