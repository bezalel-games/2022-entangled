using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoombaMove : GoombaBehaviour
{
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var playerPos = Player.position;
        var goombaPos = ThisEnemy.transform.position;
        var distance = Vector2.Distance(playerPos, goombaPos);
        
        if (distance < ThisEnemy.KeepDistance)
        {
            ThisEnemy.DesiredDirection =  goombaPos - playerPos;
        }
        else if (ThisEnemy.CanAttack && distance <= ThisEnemy.AttackDistance)
        {
            ThisEnemy.DesiredDirection = Vector2.zero;
            animator.SetTrigger("Attack");
        }
        else
        {
            ThisEnemy.DesiredDirection = GetFlockingDirection();
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        animator.ResetTrigger("Attack");
    }
}
