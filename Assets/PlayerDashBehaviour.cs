using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class PlayerDashBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        SetSpeedMultiplier(animator, stateInfo, "dash speed", GameManager.PlayerController.DashTime);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
    
    protected void SetSpeedMultiplier(Animator animator, AnimatorStateInfo stateInfo, string parameterName, float wantedTime)
    {
        var mult = animator.GetFloat(parameterName);
        var animationLength = stateInfo.length;

        if(animationLength == 0) return;
        
        var newMult = (animationLength * mult) / wantedTime;
        animator.SetFloat(parameterName, newMult);
    }
    
    // t * m = wt -> m = wt / t
}
