using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterIdle : ShooterBehaviour
{
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Move",true); 
    }
}
