using System;
using Cards;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CardUI : MonoBehaviour
    {
        #region Serialized Fields
        
        [SerializeField] private string _titleFormat;
        [SerializeField] private string _buffFormat;
        [SerializeField] private string _debuffFormat;
        [SerializeField] private RarityColors _rarityColors;

        #endregion
        #region Non-Serialized Fields

        private Image _buffImage;
        private Image _debuffImage;
        private TextMeshProUGUI _buffText;
        private TextMeshProUGUI _debuffText;
        private Image _rarityIdentifier;

        #endregion

        #region Properties

        public Card Card
        {
            set
            {
                // value.TitleString(_titleFormat);
                _buffText.text = value.BuffString(_buffFormat);
                _debuffText.text = value.DebuffString(_debuffFormat);
                _rarityIdentifier.color = _rarityColors[value.Rarity];
            }
        }

        #endregion

        #region Function Events

        private void Awake()
        {
            var buffObject = transform.GetChild(0);
            _buffImage = buffObject.GetComponent<Image>();
            _buffText = buffObject.GetComponentInChildren(typeof(TextMeshProUGUI), true) as TextMeshProUGUI;
            var debuffObject = transform.GetChild(1);
            _debuffImage = debuffObject.GetComponent<Image>();
            _debuffText = debuffObject.GetComponentInChildren(typeof(TextMeshProUGUI), true) as TextMeshProUGUI;
            _rarityIdentifier = transform.GetChild(2).GetComponent<Image>();
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion
        
        #region Class

        [Serializable]
        public class RarityColors
        {
            [SerializeField] private Color _commonColor;
            [SerializeField] private Color _rareColor;
            [SerializeField] private Color _epicColor;

            public Color this[Rarity rarity] => rarity switch
            {
                Rarity.COMMON=>_commonColor,
                Rarity.RARE=>_rareColor,
                Rarity.EPIC=>_epicColor,
                _ => throw new ArgumentOutOfRangeException(nameof(rarity), rarity, null)
            };
        }
        
        #endregion
    }
}