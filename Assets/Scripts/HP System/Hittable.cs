using UnityEngine;

public interface Hittable
{
    public void OnHit(float damage);
    public void OnHeal(float health);
    public void OnDie();
}

