using System.Collections;
using HP_System;
using Managers;
using Player;
using UnityEngine;

namespace Enemies.Boss
{
    public class Boss : YoyoOwner
    {
        #region Serialized Fields

        [Header("Boss")]
        [SerializeField] private Transform _yoyoRotationPlane;

        [SerializeField] private float _yoyoDrawDistance = 0.5f;
        [SerializeField] private float _yoyoDrawTime = 0.5f;
        [SerializeField] private GameObject _yoyoAimPivotPrefab;
        [SerializeField] private float _meleeDamage = 10;

        #endregion

        #region Non-Serialized Fields

        private Animator _animator;
        private Yoyo[] _yoyos;
        private int _idleYoyoNum;
        private int _stage;
        private static readonly int DieTrigger = Animator.StringToHash("Die");
        private static readonly int AttackingFlag = Animator.StringToHash("Attacking");

        #endregion

        #region Properties

        [Tooltip("Number of boss yoyos")] [field: SerializeField]
        public int YoyoCount { get; private set; }

        [Tooltip("The interval between throw sets in the same attack")] [field: SerializeField]
        public float ThrowSetInterval { get; private set; } = 0.2f;

        [Tooltip("The time between attacks")] [field: SerializeField]
        public float[] AttackInterval { get; private set; }

        public float NextAttackTime { get; private set; }

        #endregion

        #region Function Events

        protected override void Awake()
        {
            base.Awake();
            _animator = GetComponent<Animator>();
            _yoyos = new Yoyo[YoyoCount];
            for (int i = 0; i < YoyoCount; ++i)
            {
                var rotation = _yoyoRotationPlane.rotation * Quaternion.Euler( 0, 0, 360f * i / YoyoCount);
                var yoyoParent = Instantiate(_yoyoAimPivotPrefab, _yoyoRotationPlane.position, rotation,
                    _yoyoRotationPlane);
                _yoyos[i] = yoyoParent.GetComponentInChildren<Yoyo>();
            }

            _idleYoyoNum = YoyoCount;
        }

        protected override void Update()
        {
            base.Update();
            if (Time.time < NextAttackTime) return;
            NextAttackTime = float.PositiveInfinity;
            _animator.SetBool(AttackingFlag, true);
        }
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
                other.gameObject.GetComponent<IHittable>()?.OnHit(transform, _meleeDamage);
        }

        #endregion

        #region YoyoOwner

        public override void OnYoyoReturn()
        {
            if (++_idleYoyoNum != _yoyos.Length) return;
            _animator.SetBool(AttackingFlag, false);
            NextAttackTime = Time.time + AttackInterval[_stage];
        }

        #endregion

        #region LivingBehaviour

        public override void OnHit(Transform attacker, float damage, bool pushBack = true)
        {
            base.OnHit(attacker, damage, false);
        }

        public override void OnDie()
        {
            _animator.SetTrigger(DieTrigger);
        }
        
        public void AfterDeathAnimation()
        {
            GameManager.BossKilled();
        }

        #endregion

        #region Public Methods

        public void ShootYoyo(int yoyoIndex)
        {
            StartCoroutine(DrawAndShootYoyo(_yoyos[yoyoIndex]));
            --_idleYoyoNum;
        }

        #endregion

        #region Private Methods

        private IEnumerator DrawAndShootYoyo(Yoyo yoyo)
        {
            float startTime = Time.time;
            var yoyoTransform = yoyo.transform;
            var startPos = yoyoTransform.localPosition;
            float t;
            while ((t = (Time.time - startTime) / _yoyoDrawTime) < 1)
            {
                yoyoTransform.localPosition = startPos + Vector3.down * (_yoyoDrawDistance * t * t);
                yield return null;
            }

            yoyoTransform.localPosition = startPos + Vector3.down * _yoyoDrawDistance;
            yoyo.Shoot(yoyo.transform.parent.rotation * Vector3.up, Vector3.zero);
        }

        #endregion
    }
}