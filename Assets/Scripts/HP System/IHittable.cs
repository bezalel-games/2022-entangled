using UnityEngine;

namespace HP_System
{
    public interface IHittable
    {
        public void OnHit(Transform attacker, float damage);
        public void OnHeal(float health);
        public void OnDie();
    }
}

