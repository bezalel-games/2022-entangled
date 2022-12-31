using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class ShooterMove : MoveBehaviour<Shooter>
{

    protected override bool ShouldAttack(float distanceFromPlayer)
    {
        var validAttackDistance = 
            distanceFromPlayer <= ThisEnemy.AttackDistance && distanceFromPlayer > ThisEnemy.KeepDistance / 2;
        return ThisEnemy.CanAttack && validAttackDistance;
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
