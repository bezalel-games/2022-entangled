using Effects;
using Managers;
using UnityEngine;

namespace Enemies
{
    public class SpiritualBarrier : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private Material _regularMaterial;
        [SerializeField] private Material _ghostMaterial;

        #endregion

        #region Properties

        public bool Active
        {
            get => gameObject.activeSelf;
            set
            {
                if (gameObject.activeSelf == value) return;
                if (!value)
                    GameManager.PlayEffect(transform.position, Effect.EffectType.SHIELD_BREAK);
                gameObject.SetActive(value);
            }
        }

        #endregion

        #region Non-Serialized Fields

        private SpriteRenderer _renderer;

        #endregion

        #region Function Events

        private void Awake()
        {
            _renderer = GetComponentInParent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            _renderer.material = _ghostMaterial;
        }

        private void OnDisable()
        {
            _renderer.material = _regularMaterial;
        }

        #endregion
    }
}