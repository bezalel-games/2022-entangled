using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoombaIdle : GoombaBehaviour
{
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //uncomment lines if we don't want the goomba to follow all the time
        
        // var playerPos = _player.position;
        // var goombaPos = _goomba.transform.position;
        // var distance = Vector2.Distance(playerPos, goombaPos);
        //
        // if (distance <= _goomba.FollowDistance)
        // {
        animator.SetBool("Move",true);
        // }
    }
}
