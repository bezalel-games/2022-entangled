using HP_System;

namespace Player
{
    public abstract class YoyoOwner : LivingBehaviour
    {
        public virtual void OnSuccessfulHit()
        {
        }

        public virtual void OnPrecision()
        {
        }
        
        public virtual void OnYoyoReturn()
        {
        }
    }
}