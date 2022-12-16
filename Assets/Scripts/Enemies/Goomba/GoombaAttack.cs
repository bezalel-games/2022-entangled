using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;
using UnityEngine.Animations;

public class GoombaAttack : GoombaBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        
        SetSpeedMultiplier(animator, stateInfo, "Attack Speed", ThisEnemy.AttackTime);

        ThisEnemy.DesiredDirection = Player.position - ThisEnemy.transform.position;
        ThisEnemy.Attack((() => { animator.SetTrigger("Idle"); }));
    }
    
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ThisEnemy.CanAttack = false;
        ThisEnemy.DelayInvoke(() => { ThisEnemy.CanAttack = true;}, ThisEnemy.AttackCooldown);
        animator.ResetTrigger("Idle");
    }
}
