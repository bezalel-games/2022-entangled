using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Enemies
{
    public class MoveBehaviour<T> : EnemyBehaviour<T> where T : Enemy
    {
        
        #region Fields
        
        private readonly float wallRaycastDistance = 1;
        private readonly float enemyCastDistance = 1;
        private int wallMask = 0;
        
        #endregion

        #region StateMachineBehaviour Methods

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            wallMask = LayerMask.GetMask("Walls");

            ThisEnemy.DesiredDirection = GetFlockingDirection();
        }
        
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.ResetTrigger("Attack");
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            
            if(!AtFrameRate) return;
        
            var playerPos = Player.position;
            var goombaPos = ThisEnemy.transform.position;
            var distance = Vector2.Distance(playerPos, goombaPos);
            
            if (ShouldAttack(distance))
            {
                animator.SetTrigger("Attack");
            }
            else
            {
                ThisEnemy.DesiredDirection = GetFlockingDirection();
            }
        }

        #endregion

        #region Flocking
        
        protected virtual Vector2 GetToPlayerDirection()
        {
            var playerPos = Player.position;
            var shooterPos = ThisEnemy.transform.position;
            var distance = Vector2.Distance(playerPos, shooterPos);
        
            var dir = ThisEnemy.DesiredDirection;
            var towardsPlayerDirection = (playerPos - shooterPos);
        
            var t = (ThisEnemy.KeepDistance + distance) / (ThisEnemy.KeepDistance + ThisEnemy.AttackDistance);
            var towardPlayerCoefficient = (t - 0.5f);

            var keepDistanceMult = towardPlayerCoefficient < 0 ? 2 : 1;
            return keepDistanceMult * (towardPlayerCoefficient * towardsPlayerDirection).normalized;
        }
        
        protected virtual bool ShouldAttack(float distanceFromPlayer)
        {
            var validAttackDistance = 
                distanceFromPlayer <= ThisEnemy.AttackDistance && distanceFromPlayer > ThisEnemy.KeepDistance / 2;
            return ThisEnemy.CanAttack && validAttackDistance;
        }

        protected Vector2 GetFlockingDirection()
        {
            var direction = Vector2.zero;
            
            var position = ThisEnemy.transform.position;
            
            var results = new Collider2D[10];
            var size = Physics2D.OverlapCircleNonAlloc(position, enemyCastDistance, results, Enemy.Layer);
            
            if (size > 0)
            {
                var enemies = new List<Enemy>();
                foreach (var collider in results)
                {
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
            direction += ThisEnemy.AwayFromWallWeight * GetMoveFromWallDirection();

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
                var factor = (dir.magnitude >= magnitudeSums) ? 1 : (1 - dir.magnitude / magnitudeSums);  
                sepDirection += factor * dir.normalized;
            }

            return sepDirection.normalized;
        }

        private Vector2 GetMoveFromWallDirection()
        {
            var direction = Vector2.zero;
            var enemyPosition = ThisEnemy.transform.position;

            foreach (var dir in new Vector2[] {Vector2.up, Vector2.down, Vector2.left, Vector2.right})
            {
                var wall = Physics2D.Raycast(
                    enemyPosition, 
                    dir, 
                    wallRaycastDistance, 
                    wallMask);
                
                if(wall.collider == null) continue;
                
                var factor = 1 - (wall.distance / wallRaycastDistance);
                direction = -1 * factor * dir;
            }

            return direction;
        }

        #endregion
    }
}