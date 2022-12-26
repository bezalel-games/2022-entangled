using System;
using System.Collections.Generic;
using Enemies;
using UnityEngine;
using UnityEngine.Search;
using Utils;

public class Shooter : Enemy
{
    #region Serialized Fields

    [Header("Shooter")] 
    [SerializeField] private float shootCooldown;
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private float _projectileSpeed;

    #endregion

    #region Non-Serialized Fields

    private Stack<Projectile> _projectilePool = new Stack<Projectile>();

    #endregion

    #region Properties

    [field: SerializeField] public int ShotsPerAttack { get; private set; }
    public bool CanShoot { get; private set; }

    #endregion

    #region Function Events

    protected override void Awake()
    {
        base.Awake();
        CanShoot = true;
    }

    #endregion

    #region Public Methods

    public void Shoot(Vector2 dir)
    {
        var projPos = transform.position;
        
        Projectile proj;
        if (_projectilePool.Count > 0)
        {
            proj = _projectilePool.Pop();
            proj.transform.position = projPos;
            proj.gameObject.SetActive(true);
        }
        else
        {
            proj = Instantiate(_projectilePrefab, projPos, Quaternion.identity);
        }
        
        proj.Damage = _damage;
        proj.Speed = _projectileSpeed;
        proj.Direction = dir;
        proj.OnDisappear = () =>
        {
            /*
             * since setActive(false) calls OnBecameInvisible, we must start by null-ing OnDisappear so it won't
             * get called twice
             */
            proj.OnDisappear = null;
            proj.gameObject.SetActive(false);
            _projectilePool.Push(proj);
        };

        CanShoot = false;
        DelayInvoke(() => { CanShoot = true; }, shootCooldown);
    }

    #endregion

    #region Private Methods
    
    protected override void Move()
    {
        if (!Attacking)
        {
            base.Move();
        }
    }

    #endregion
}