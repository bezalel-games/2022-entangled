using HP_System;

namespace Player
{
    public abstract class YoyoOwner : LivingBehaviour
    {
        public abstract void OnSuccessfulHit();

        public abstract void OnPrecision();
    }
}