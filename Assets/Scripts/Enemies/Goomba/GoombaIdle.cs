using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoombaIdle : GoombaBehaviour
{
    // public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //     animator.SetBool("Move",true);
    // }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Move",true);
    }
}
