using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;
using UnityEngine.Animations;

public class GoombaAttack : AttackBehaviour<Goomba>
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        ThisEnemy.DesiredDirection = Player.position - ThisEnemy.transform.position;
        ThisEnemy.Attack((() => { animator.SetTrigger("Idle"); ThisEnemy.Stop();}));
    }
}
