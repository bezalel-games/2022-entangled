using Enemies;
using UnityEngine;

public class ShooterBehaviour : EnemyBehaviour<Shooter>
{
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