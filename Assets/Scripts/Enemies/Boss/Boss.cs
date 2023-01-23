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

        [SerializeField] private int _numberOfPhases = 2;

        [Header("Phase 1")]
        [SerializeField] private float _yoyoDrawDistance = 0.5f;

        [SerializeField] private float _yoyoDrawTime = 0.5f;
        [SerializeField] private GameObject _yoyoAimPivotPrefab;
        [SerializeField] private float _meleeDamage = 10;

        [Header("Phase 2")]
        [SerializeField] private float _spinSpeed;

        [SerializeField] private Bomb _bombPrefab;
        [SerializeField] private float _bombThrowSpeed = 1;
        [SerializeField] private Vector3 _bombOffset = new Vector3(0, -1, -0.5f);

        #endregion

        #region Non-Serialized Fields

        private Animator _animator;
        private Yoyo[] _yoyos;
        private GameObject _shield;
        private int _idleYoyoNum;
        private static readonly int DieTrigger = Animator.StringToHash("Die");
        private static readonly int AttackingFlag = Animator.StringToHash("Attacking");
        private static readonly int PhaseNumber = Animator.StringToHash("Phase");
        private Bomb _bomb;
        private int _phase = 1;

        #endregion

        #region Properties

        private int Phase
        {
            get => _phase;
            set
            {
                _phase = value;
                Animator.SetInteger(PhaseNumber, _phase);
            }
        }

        [Tooltip("Number of boss yoyos")] [field: SerializeField]
        public int YoyoCount { get; private set; }

        [Tooltip("The interval between throw sets in the same attack")] [field: SerializeField]
        public float ThrowSetInterval { get; private set; } = 0.2f;

        [Tooltip("The time between attacks")] [field: SerializeField]
        public float[] AttackInterval { get; private set; }

        public float NextAttackTime { get; private set; }
        public int MinYoyoInRoom { get; private set; } = -1;
        public int MaxYoyoInRoom { get; private set; } = -1;

        public override float Hp
        {
            get => base.Hp;
            protected set
            {
                base.Hp = value;
                if (value < (1 - (Phase / (float)_numberOfPhases)) * MaxHp)
                    ++Phase;
            }
        }

        #endregion

        #region Function Events

        protected override void Awake()
        {
            base.Awake();
            _animator = GetComponent<Animator>();
            _shield = _yoyoRotationPlane.GetChild(0).gameObject;
            _yoyos = new Yoyo[YoyoCount];
            for (int i = 0; i < YoyoCount; ++i)
            {
                var degrees = 360f * i / YoyoCount;
                var rotation = _yoyoRotationPlane.rotation * Quaternion.Euler(0, 0, degrees);
                var yoyoParent = Instantiate(_yoyoAimPivotPrefab, _yoyoRotationPlane.position, rotation,
                    _yoyoRotationPlane);
                _yoyos[i] = yoyoParent.GetComponentInChildren<Yoyo>();
                if (MinYoyoInRoom == -1 && degrees >= 90) MinYoyoInRoom = i;
                if (degrees is >= 90 and <= 270) MaxYoyoInRoom = i;
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
            NextAttackTime = Time.time + AttackInterval[Phase - 1];
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

        protected override void HitShake() => CameraManager.EnemyHitShake();

        #endregion

        #region Public Methods

        public void ShootYoyo(int yoyoIndex)
        {
            StartCoroutine(DrawAndShootYoyo(_yoyos[yoyoIndex]));
            --_idleYoyoNum;
        }

        public void SpinYoyos(float t)
        {
            _yoyoRotationPlane.localRotation *= Quaternion.AngleAxis(t * _spinSpeed, transform.forward);
        }

        public void CreateBomb()
        {
            _bomb = Instantiate(_bombPrefab, transform.position + _bombOffset, Quaternion.identity);
            StartCoroutine(CreateBombAnimation());
        }

        public void ThrowBomb(Vector3 targetPosition)
        {
            _bomb.GetComponent<Rigidbody2D>()
                .AddForce((targetPosition - transform.position).normalized * _bombThrowSpeed, ForceMode2D.Impulse);
            _bomb.Active = true;
            Animator.SetBool(AttackingFlag, false);
            NextAttackTime = Time.time + AttackInterval[Phase - 1];
        }

        public void ShieldActive(bool isActive) => _shield.SetActive(isActive);

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
            yoyo.Shoot(yoyoTransform.up, Vector3.zero);
        }

        private IEnumerator CreateBombAnimation()
        {
            var bombTransform = _bomb.transform;
            var startTime = Time.time;
            var startScale = bombTransform.localScale;
            bombTransform.localScale = 0.0001f * startScale;
            float t = 0;
            while (t < 1)
            {
                yield return null;
                t = Time.time - startTime;
                bombTransform.localScale = startScale * t;
            }

            bombTransform.localScale = startScale;
        }

        #endregion
    }
}