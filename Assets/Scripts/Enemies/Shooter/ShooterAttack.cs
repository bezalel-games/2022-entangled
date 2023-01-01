using Enemies;
using UnityEngine;

public class ShooterAttack : AttackBehaviour<Shooter>
{
    private int shootCounter;
    private Vector2 dir;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        shootCounter = 0;
        dir = Player.position - ThisEnemy.transform.position;
        ThisEnemy.DesiredDirection = dir;
    }
    
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        
        if (ThisEnemy.CanShoot)
        {
            ThisEnemy.Shoot(dir);
            shootCounter++;
        }

        if (shootCounter >= ThisEnemy.ShotsPerAttack)
        {
            animator.SetTrigger(Idle);
        }
    }
}
