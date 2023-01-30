using Audio;
using Cards.Buffs.Components;
using Cards.CardElementClasses;
using Cards.Factory;
using Player;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace Cards.Buffs.ActiveBuffs
{
    public class ExplosiveYoyo : Buff
    {
        #region Fields

        private readonly Explosion _explosion;
        private Yoyo _yoyo;

        #endregion
        
        #region Properties

        public float ExplosionRadius { get; set; }
        public float Damage { get; set; } = 8;

        #endregion

        #region Buff Implementation
        
        public override BuffType Type => BuffType.EXPLOSIVE_YOYO;

        public override void Apply(PlayerController playerController)
        {
            _yoyo = playerController.Yoyo;
            _yoyo.ReachedThrowPeak += BlowUpYoyo;
            _yoyo.ExplosiveYoyo = this;
        }

        #endregion

        #region Constructor

        public ExplosiveYoyo(CardElementClassAttributes attributes, Rarity rarity, float explosionRadius,
            Explosion explosionPrefab)
            : base(attributes, rarity)
        {
            ExplosionRadius = explosionRadius;
            _explosion = explosionPrefab;
        }

        #endregion

        #region Private Methods

        private void BlowUpYoyo()
        {
            ((IAudible<YoyoSounds>) _yoyo).PlayOneShot(YoyoSounds.EXPLOSION);
            var explosion = Object.Instantiate(_explosion, _yoyo.transform);
            explosion.Radius = ExplosionRadius;
            explosion.Damage = Damage;
        }

        #endregion
    }
}