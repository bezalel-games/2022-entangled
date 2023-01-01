using System;
using UnityEngine;

namespace Enemies
{
    public class Fumer : Enemy
    {
        #region Serialized Fields

        [Header("Fumer")]
        [SerializeField] private float _Speed;

        #endregion

        #region Non-Serialized Fields

        private ParticleSystem _particleSystem;

        #endregion

        #region Properties

        public bool PreparingAttack { get; set; } = false;

        #endregion

        #region Function Events

        protected override void Awake()
        {
            base.Awake();
            _particleSystem = GetComponent<ParticleSystem>();
        }

        #endregion

        #region Public Methods

        public void Attack(Action onEnd)
        {
            // _particleSystem.
            DelayInvoke(() =>
            {
                Attacking = false;
                onEnd?.Invoke();
            }, AttackTime);
        }

        #endregion

        #region Private Methods

        protected override void Move()
        {
            if (!Attacking || IsDead)
            {
                base.Move();
            }
        }

        #endregion
    }
}