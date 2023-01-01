using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class GoombaIdle : IdleBehaviour<Goomba>
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Move",true);
    }
}
