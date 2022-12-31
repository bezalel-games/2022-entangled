using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class GoombaMove : MoveBehaviour<Goomba>
{

    protected override bool ShouldAttack(float distanceFromPlayer)
    {
        var validAttackDistance = 
            distanceFromPlayer <= ThisEnemy.AttackDistance && distanceFromPlayer > ThisEnemy.KeepDistance / 2;
        return ThisEnemy.CanAttack && validAttackDistance;
    }

    protected override Vector2 GetToPlayerDirection()
    {
        return (Player.position - ThisEnemy.transform.position).normalized;
    }
}
