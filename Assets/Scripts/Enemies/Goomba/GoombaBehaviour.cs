using Enemies;
using UnityEngine;

public class GoombaBehaviour : EnemyBehaviour<Goomba>
{
    protected override Vector2 GetToPlayerDirection()
    {
        return (Player.position - ThisEnemy.transform.position).normalized;
    }
}
