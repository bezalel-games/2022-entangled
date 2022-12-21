using Cards.Buffs.Components;
using Cards.CardElementClasses;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cards.Buffs.ActiveBuffs
{
    public class ExplosiveYoyo : Buff
    {
        private float _explosionRadius;
        private readonly Explosion _explosion;

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

        public ExplosiveYoyo(CardElementClassAttributes attributes, Rarity rarity, float explosionRadius,
            Explosion explosionPrefab)
            : base(attributes, rarity)
        {
            _explosionRadius = explosionRadius;
            _explosion = explosionPrefab;
        }

        #endregion

        #region Private Methods

        private void BlowUpYoyo(InputActionPhase phase)
        {
            if (phase is not InputActionPhase.Started || _yoyo.State is Yoyo.YoyoState.IDLE) return;
            var explosion = Object.Instantiate(_explosion, _yoyo.transform);
            explosion.Radius = _explosionRadius;
            if (_yoyo.State is Yoyo.YoyoState.PRECISION)
                _yoyo.CancelPrecision();
        }

        #endregion
    }
}