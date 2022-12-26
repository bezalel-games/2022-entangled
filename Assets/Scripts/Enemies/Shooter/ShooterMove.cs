using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class ShooterMove : MoveBehaviour<Shooter>
{

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        
        if(!AtFrameRate) return;
        
        var playerPos = Player.position;
        var shooterPos = ThisEnemy.transform.position;
        var distance = Vector2.Distance(playerPos, shooterPos);
        
        if (distance < ThisEnemy.KeepDistance)
        {
            ThisEnemy.DesiredDirection =  shooterPos - playerPos;
        }
        else if (ThisEnemy.CanAttack && distance <= ThisEnemy.AttackDistance)
        {
            ThisEnemy.Stop();
            animator.SetTrigger("Attack");
        }
        else
        {
            ThisEnemy.DesiredDirection = GetFlockingDirection();
        }    
    }
    
    protected override Vector2 GetToPlayerDirection()
    {
        var playerPos = Player.position;
        var shooterPos = ThisEnemy.transform.position;
        var distance = Vector2.Distance(playerPos, shooterPos);
        
        var dir = ThisEnemy.DesiredDirection;
        var towardsPlayerDirection = (playerPos - shooterPos);
        
        var t = (ThisEnemy.KeepDistance + distance) / (ThisEnemy.KeepDistance + ThisEnemy.AttackDistance);
        var towardPlayerCoefficient = (t - 0.5f);

        var keepDistanceMult = towardPlayerCoefficient < 0 ? 2 : 1;
        return keepDistanceMult * (towardPlayerCoefficient * towardsPlayerDirection).normalized;
    }
}
