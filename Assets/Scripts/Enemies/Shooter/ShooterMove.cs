using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterMove : ShooterBehaviour
{

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var playerPos = Player.position;
        var shooterPos = ThisEnemy.transform.position;
        var distance = Vector2.Distance(playerPos, shooterPos);
        
        if (distance < ThisEnemy.KeepDistance)
        {
            ThisEnemy.DesiredDirection =  shooterPos - playerPos;
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
}
