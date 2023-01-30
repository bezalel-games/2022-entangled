using System;
using Audio;
using Cards;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Utils.Coroutines;

namespace UI
{
    public class CardUI : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private string _buffFormat;
        [SerializeField] private string _debuffFormat;
        [SerializeField] private RarityIdentifierSprites _rarityIdentifierSprites;

        #endregion

        #region Non-Serialized Fields

        private Image _buffImage;
        private Image _debuffImage;
        private Image _borderGlow;
        private Image _circleGlow;
        private TextMeshProUGUI _buffText;
        private TextMeshProUGUI _debuffText;
        private Image _rarityIdentifier;

        private Rarity _buffRarity;
        private Rarity _debuffRarity;
        private RectTransform _rectTransform;

        #endregion

        #region Properties

        public Card Card
        {
            set
            {
                _buffText.text = value.BuffString(_buffFormat);
                _debuffText.text = value.DebuffString(_debuffFormat);
                _rarityIdentifier.sprite = _rarityIdentifierSprites[value.Rarity];
                AssignImage(_buffImage, value.BuffSprite);
                AssignImage(_debuffImage, value.DebuffSprite);

                _buffRarity = value.BuffRarity;
                _debuffRarity = value.DebuffRarity;
                SetGlowColor();
            }
        }

        #endregion

        #region Function Events

        private void Awake()
        {
            var borderGlowObject = transform.GetChild(0);
            var circleGlowObject = transform.GetChild(5);
            _borderGlow = borderGlowObject.GetComponentInChildren(typeof(Image), true) as Image;
            _circleGlow = circleGlowObject.GetComponentInChildren(typeof(Image), true) as Image;

            var buffObject = transform.GetChild(1);
            _buffImage = buffObject.GetComponentInChildren(typeof(Image), true) as Image;
            _buffText = buffObject.GetComponentInChildren(typeof(TextMeshProUGUI), true) as TextMeshProUGUI;
            var debuffObject = transform.GetChild(2);
            _debuffImage = debuffObject.GetComponentInChildren(typeof(Image), true) as Image;
            _debuffText = debuffObject.GetComponentInChildren(typeof(TextMeshProUGUI), true) as TextMeshProUGUI;
            _rarityIdentifier = transform.GetChild(3).GetComponent<Image>();
            
            _rectTransform = GetComponent<RectTransform>();
        }

        #endregion

        #region Public Methods

        public void Show(float duration, bool reverse = false)
        {
            AudioManager.PlayOneShot(SoundType.SFX, (int) SfxSounds.CARDS_IN_OUT);
            float start = reverse ? -1000 : 1000;
            const float end = 0;
            void EasedShow(float t)
            {
                var pos = _rectTransform.localPosition;
                pos.y = Mathf.Lerp(start, end, Mathf.Sqrt(t));
                _rectTransform.localPosition = pos;
            }

            StartCoroutine(Interpolate(EasedShow, duration));
        }

        public void Hide(float duration, bool reverse = false)
        {
            AudioManager.PlayOneShot(SoundType.SFX, (int) SfxSounds.CARDS_IN_OUT);
            float end = reverse ? 1000 : -1000;
            const float start = 0;
            void EasedHide(float t)
            {
                var pos = _rectTransform.localPosition;
                pos.y = Mathf.Lerp(start, end, Mathf.Pow(t, 2));
                _rectTransform.localPosition = pos;
            }

            StartCoroutine(Interpolate(EasedHide, duration));
        }
        
        #endregion

        #region Private Methods

        private void SetGlowColor()
        {
            Color c = CardManager.BuffGlowColor[_buffRarity, _debuffRarity];
            _borderGlow.color = c;
            _circleGlow.color = c;
        }

        private static void AssignImage(Image image, Sprite sprite)
        {
            if (sprite == null)
            {
                image.enabled = false;
                return;
            }

            image.enabled = true;
            image.sprite = sprite;
        }

        #endregion

        #region Class

        [Serializable]
        public class RarityIdentifierSprites
        {
            [SerializeField] private Sprite _common;
            [SerializeField] private Sprite _rare;
            [SerializeField] private Sprite _epic;

            public Sprite this[Rarity rarity] => rarity switch
            {
                Rarity.COMMON => _common,
                Rarity.RARE => _rare,
                Rarity.EPIC => _epic,
                _ => throw new ArgumentOutOfRangeException(nameof(rarity), rarity, null)
            };
        }

        #endregion
    }
}