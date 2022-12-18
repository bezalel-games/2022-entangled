using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class ShooterAttack : ShooterBehaviour
{
    private int shootCounter;
    private Vector2 dir;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        shootCounter = 0;
        dir = Player.position - ThisEnemy.transform.position;
    }
    
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (ThisEnemy.CanShoot)
        {
            ThisEnemy.Shoot(dir);
            shootCounter++;
        }

        if (shootCounter >= ThisEnemy.ShotsPerAttack)
        {
            animator.SetTrigger("Idle");
        }
    }
    
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Enemy.NumberOfAttacking--;
        ThisEnemy.CanAttack = false;
        ThisEnemy.DelayInvoke(() => { ThisEnemy.CanAttack = true;}, ThisEnemy.AttackCooldown);
        animator.ResetTrigger("Idle");
    }
}
