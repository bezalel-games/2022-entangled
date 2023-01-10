using System;
using Player;
using UnityEngine;

namespace Enemies.Boss
{
    public class Boss : YoyoOwner
    {
        #region Serialized Fields

        [Header("Boss")]
        [SerializeField] private Transform _yoyoRotationPlane;

        #endregion

        #region Non-Serialized Fields

        private Animator _animator;
        private Yoyo[] _yoyos;
        private int _idleYoyoNum;
        private int _stage;
        private static readonly int DieTrigger = Animator.StringToHash("Die");
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");

        #endregion

        #region Properties

        public int YoyoCount => _yoyos.Length;

        [Tooltip("The interval between throw sets in the same attack")] [field: SerializeField]
        public float ThrowSetInterval { get; private set; } = 0.2f;

        [Tooltip("The time between attacks")] [field: SerializeField]
        public float[] AttackInterval { get; private set; }
        
        public float NextAttackTime { get; private set; }

        #endregion

        #region Events

        public event Action<int> Idle;

        #endregion

        #region Function Events

        protected override void Awake()
        {
            base.Awake();
            _animator = GetComponent<Animator>();
            _yoyos = new Yoyo[_yoyoRotationPlane.childCount];
            _idleYoyoNum = _yoyos.Length;
            for (int i = 0; i < _idleYoyoNum; ++i)
                _yoyos[i] = _yoyoRotationPlane.GetChild(i).GetComponentInChildren<Yoyo>();
        }

        protected override void Update()
        {
            base.Update();
            if (Time.time < NextAttackTime) return;
            NextAttackTime = float.PositiveInfinity;
            _animator.SetTrigger(AttackTrigger);
        }

        #endregion

        #region YoyoOwner

        public override void OnYoyoReturn()
        {
            if (++_idleYoyoNum != _yoyos.Length) return;
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

        #endregion

        #region Public Methods

        public void ShootYoyo(int yoyoIndex)
        {
            var yoyo = _yoyos[yoyoIndex];
            yoyo.Shoot(yoyo.transform.parent.rotation * Vector3.up, Vector3.zero);
            --_idleYoyoNum;
        }

        #endregion

        #region Private Methods

        #endregion
    }
}