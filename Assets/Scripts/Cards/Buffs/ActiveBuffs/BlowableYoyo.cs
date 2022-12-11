using Player;
using UnityEngine.InputSystem;

namespace Cards.Buffs.ActiveBuffs
{
    public class BlowableYoyo : Buff 
    {
        protected override void ApplyBuff(PlayerController playerController)
        {
            playerController.QuickShotEvent += BlowUpYoyo;
        }

        private void BlowUpYoyo(InputActionPhase phase)
        {
            if (phase is not InputActionPhase.Started) return;
            
        }
    }
}