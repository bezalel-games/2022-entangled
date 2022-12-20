using System;
using Cards.CardElementClasses;
using Player;
using UnityEngine.InputSystem;

namespace Cards.Buffs.ActiveBuffs
{
    public class ExplosiveYoyo : Buff
    {
        #region Fields

        private Yoyo _yoyo;

        #endregion

        #region Buff Implementation

        public override void Apply(PlayerController playerController)
        {
            _yoyo = playerController.Yoyo;
            playerController.QuickShotEvent += BlowUpYoyo;
        }

        #endregion

        #region Constructor

        public ExplosiveYoyo(CardElementClassAttributes attributes, Rarity rarity) : base(attributes, rarity)
        {
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