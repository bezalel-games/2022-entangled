using System;
using Managers;
using UnityEngine;

namespace Enemies
{
    public class Goomba : Enemy

    {
        #region Serialized Fields

        [Header("Goomba")]
        [SerializeField] private float _attackSpeed;

        #endregion

        #region Non-Serialized Fields

        private bool _following;

        #endregion

        #region Properties

        public bool PreparingAttack { get; set; } = false;
        public bool StunningAttack { get; set; } = false;

        public float StunDuration { get; set; } = 1;

        #endregion

        #region Function Events

        protected override void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (Attacking && StunningAttack)
                {
                    GameManager.PlayerController.Stun(StunDuration);
                }
                base.OnCollisionEnter2D(other);
            }
        }

        #endregion

        #region Public Methods

        public void Attack(Action onEnd)
        {
            DelayInvoke((() =>
            {
                Attacking = false;
                onEnd?.Invoke();
            }), AttackTime);
        }

        #endregion

        #region Private Methods

        protected override void Move()
        {
            if (!Attacking || IsDead || Frozen)
            {
                base.Move();
            }
            else
            {
                if (!PreparingAttack)
                    Rigidbody.velocity = DesiredDirection * _attackSpeed;
            }
        }

        #endregion
    }
}