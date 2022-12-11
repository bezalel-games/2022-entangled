using System;
using Player;
using Player.Yoyo;
using UnityEngine.InputSystem;

namespace Cards.Buffs.ActiveBuffs
{
    public class BlowableYoyo : IBuff 
    {
        private Yoyo _yoyo;

        public void Apply(PlayerController playerController)
        {
            _yoyo = playerController.Yoyo;
            playerController.QuickShotEvent += BlowUpYoyo;
        }

        private void BlowUpYoyo(InputActionPhase phase)
        {
            if (phase is not InputActionPhase.Started) return;
            throw new NotImplementedException("what is the hit interface?");
        }
    }
}