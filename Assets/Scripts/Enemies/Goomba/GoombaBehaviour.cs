using Managers;
using UnityEngine;

public class GoombaBehaviour : StateMachineBehaviour
{
    private bool _init;
    protected Transform _player;
    protected Goomba _goomba;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_init)
        {
            Init(animator);
        }
    }
    
    private void Init(Animator animator)
    {
        _player = GameManager.PlayerTransform;
        _goomba = animator.GetComponent<Goomba>();
        _init = true;
    }
}
