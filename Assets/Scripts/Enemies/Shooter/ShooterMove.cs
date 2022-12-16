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
        else if (distance <= ThisEnemy.AttackDistance)
        {
            ThisEnemy.DesiredDirection = Vector2.zero;
            animator.SetTrigger("Attack");
        }
        else
        {
            ThisEnemy.DesiredDirection = GetDirection(distance);
        }    
    }

    //TODO: add flocking
    private Vector2 GetDirection(float distance)
    {
        var dir = ThisEnemy.DesiredDirection;
        var towardsPlayerDirection = (Player.position - ThisEnemy.transform.position);
        
        var t = (ThisEnemy.KeepDistance + distance) / (ThisEnemy.AttackDistance + distance);
        var towardPlayerCoefficient = (t - 0.5f);

        return (towardPlayerCoefficient * towardsPlayerDirection).normalized;
    }
}
