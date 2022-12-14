using System;
using Player;
using UnityEngine.InputSystem;

namespace Cards.Buffs.ActiveBuffs
{
    public class ExplosiveYoyo : IBuff
    {
        #region Fields

        private Yoyo _yoyo;

        #endregion

        #region ICardProperty Implementation

        public string Name => "Spectral Pulse";

        public string Description =>
            "Create a powerful pulse from your weapon by pressing R1 when you are not holding it";

        public string Rarity => "Normal";

        #endregion

        #region IBuff Implementation

        public void Apply(PlayerController playerController)
        {
            _yoyo = playerController.Yoyo;
            playerController.QuickShotEvent += BlowUpYoyo;
        }

        #endregion

        #region Private Methods

        private void BlowUpYoyo(InputActionPhase phase)
        {
            if (phase is not InputActionPhase.Started) return;
            throw new NotImplementedException("what is the hit interface?");
        }

        #endregion
    }
}