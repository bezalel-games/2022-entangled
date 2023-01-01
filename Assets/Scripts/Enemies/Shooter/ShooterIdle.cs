using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class ShooterIdle : IdleBehaviour<Shooter>
{
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Move",true); 
    }
}
