using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class GoombaMove : MoveBehaviour<Goomba>
{
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        
        if(!AtFrameRate) return;
        
        var playerPos = Player.position;
        var goombaPos = ThisEnemy.transform.position;
        var distance = Vector2.Distance(playerPos, goombaPos);

        var validAttackDistance = distance <= ThisEnemy.AttackDistance && distance > ThisEnemy.KeepDistance / 2;
        if (ThisEnemy.CanAttack && validAttackDistance)
        {
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
    
    protected override Vector2 GetToPlayerDirection()
    {
        return (Player.position - ThisEnemy.transform.position).normalized;
    }
}
