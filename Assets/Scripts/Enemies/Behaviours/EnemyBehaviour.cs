using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Enemies
{
    public abstract class EnemyBehaviour<T> : StateMachineBehaviour where T : Enemy
    {
        private bool _init;
        protected Transform Player;
        protected T ThisEnemy;

        private readonly int frameRate = 20;
        private int frameCount;

        protected bool AtFrameRate => frameCount % frameRate == 0;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!_init)
            {
                Init(animator);
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            frameCount++;
        }

        private void Init(Animator animator)
        {
            Player = GameManager.PlayerTransform;
            
            ThisEnemy = animator.GetComponent<T>() ?? animator.GetComponentInParent<T>();

            _init = true;
        }
        
        protected void SetSpeedMultiplier(Animator animator, AnimatorStateInfo stateInfo, string parameterName, float wantedTime)
        {
            var mult = animator.GetFloat(parameterName);
            var animationLength = stateInfo.length;

            if(animationLength == 0) return;
        
            var newMult = wantedTime / (animationLength * mult);
            animator.SetFloat(parameterName, newMult);
        }

        protected Vector2 GetFlockingDirection()
        {
            var direction = Vector2.zero;
            
            var position = ThisEnemy.transform.position;
            
            var results = new RaycastHit2D[10];
            var size = Physics2D.CircleCastNonAlloc(position, 5, Vector2.zero, results, 0, Enemy.Layer);
            
            if (size > 0)
            {
                var enemies = new List<Enemy>();
                foreach (var hit in results)
                {
                    var collider = hit.collider;
                    if(collider == null) continue;
                    var enemy = collider.GetComponent<Enemy>() ?? collider.GetComponentInParent<Enemy>(); 
                    enemies.Add(enemy);
                }

                if (enemies.Count > 0)
                {
                    direction += ThisEnemy.SeparationWeight * GetSeparationDirection(enemies);
                }
            }

            direction += ThisEnemy.ToPlayerWeight * GetToPlayerDirection();

            return direction.normalized;
        }

        private Vector2 GetSeparationDirection(List<Enemy> enemies)
        {
            var sepDirection = Vector2.zero;
            var pos = ThisEnemy.transform.position;

            float magnitudeSums = 0;
            foreach (var enemy in enemies)
            {
                magnitudeSums += (pos - enemy.transform.position).magnitude;
            }
            foreach (var enemy in enemies)
            {
                var dir = (Vector2) (pos - enemy.transform.position);
                sepDirection += dir.normalized * (1 - dir.magnitude/magnitudeSums);
            }

            return sepDirection.normalized;
        }

        protected abstract Vector2 GetToPlayerDirection();
    }
}