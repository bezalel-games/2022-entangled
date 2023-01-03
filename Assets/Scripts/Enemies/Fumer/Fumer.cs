using System;
using System.Collections.Generic;
using Managers;
using Player;
using UnityEngine;

namespace Enemies
{
    public class Fumer : Enemy
    {
        #region Serialized Fields

        [Header("Fumer")]
        [SerializeField] private float _fumeDamage = 1;
        [SerializeField] private float _fumeDamageInterval = 0.5f;

        #endregion

        #region Non-Serialized Fields

        private ParticleSystem _particleSystem;
        private readonly List<ParticleSystem.Particle> _onTriggerParticleList = new();
        private static float _nextHitMinTime = 0;

        #endregion

        #region Properties

        public bool PreparingAttack { get; set; } = false;

        #endregion

        #region Function Events

        protected override void Awake()
        {
            base.Awake();
            _particleSystem = GetComponent<ParticleSystem>();
            _particleSystem.trigger.AddCollider(GameManager.PlayerTransform);
            if (_nextHitMinTime > Time.time + _fumeDamageInterval)
                _nextHitMinTime = 0;
        }

        private void OnParticleTrigger()
        {
            if (Time.time < _nextHitMinTime)
                return;
            
            int numInside = _particleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, _onTriggerParticleList, out var colliderData);
 
            for (int i = 0; i < numInside; i++)
            {
                var player = colliderData.GetCollider(i,0)?.GetComponent<PlayerController>();
                if (player)
                {
                    player.OnHit(null, _fumeDamage);
                    _nextHitMinTime = Time.time + _fumeDamageInterval;
                    break;
                }
            }
        }

        #endregion

        #region Public Methods

        public void Attack(Action onEnd)
        {
            _particleSystem.Play();
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