using System;
using Cards;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

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

        private Rarity _buffRariy;
        private Rarity _debuffRariy;

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

                _buffRariy = value.BuffRarity;
                _debuffRariy = value.DebuffRarity;
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
        }

        #endregion

        #region Public Methods

        public void SetGlowColor()
        {
            /*
             * moving from [-d,d] to [0,1]: x/(2*d) + 0.5f
             */
            int d = (int) Rarity.EPIC;
            int value = (int) _buffRariy - (int) _debuffRariy;
            float t = (value / (2f * d)) + 0.5f;
            
            Color c = (1 - t) * CardManager.DebuffColor + t * CardManager.BuffColor;
            _borderGlow.color = c;
            _circleGlow.color = c;
        }

        #endregion

        #region Private Methods

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