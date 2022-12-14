namespace HP_System
{
    public interface IHittable
    {
        public void OnHit(float damage);
        public void OnHeal(float health);
        public void OnDie();
    }
}

