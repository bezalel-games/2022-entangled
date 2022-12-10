using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class GoombaAttack : GoombaBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        
        SetSpeedMultiplier(animator, stateInfo);

        _goomba.DesiredDirection = _player.position - _goomba.transform.position;
        _goomba.Attack((() => { animator.SetTrigger("Idle"); }));
    }
    
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Idle");
    }

    private void SetSpeedMultiplier(Animator animator, AnimatorStateInfo stateInfo)
    {
        var mult = animator.GetFloat("Attack Speed");
        var animationLength = stateInfo.length;

        if(animationLength == 0) return;
        
        var newMult = _goomba.AttackTime / (animationLength * mult);
        animator.SetFloat("Attack Speed", newMult);
    }
}
