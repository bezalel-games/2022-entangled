using System;
using System.Collections.Generic;
using Enemies;
using UnityEngine;
using UnityEngine.Search;

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
        print("shoot");
        var projPos = transform.position;// + ((Vector3) dir) * 0.5f;

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
            proj.OnCollision = () =>
            {
                proj.gameObject.SetActive(false);
                _projectilePool.Push(proj);
            };
        }
        
        proj.Damage = _damage;
        proj.Speed = _projectileSpeed;
        proj.Direction = dir;

        CanShoot = false;
        DelayInvoke(() => { CanShoot = true; }, shootCooldown);
    }

    #endregion

    #region Private Methods

    #endregion
}