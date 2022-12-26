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
        
        [SerializeField] private string _titleFormat;
        [SerializeField] private string _buffFormat;
        [SerializeField] private string _debuffFormat;
        [SerializeField] private RarityIdentifierSprites _rarityIdentifierSprites;

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
                _rarityIdentifier.sprite = _rarityIdentifierSprites[value.Rarity];
                _buffImage.sprite = value.BuffSprite;
                _debuffImage.sprite = value.DebuffSprite;
            }
        }

        #endregion

        #region Function Events

        private void Awake()
        {
            var buffObject = transform.GetChild(0);
            _buffImage = buffObject.GetComponentInChildren(typeof(Image), true) as Image;
            _buffText = buffObject.GetComponentInChildren(typeof(TextMeshProUGUI), true) as TextMeshProUGUI;
            var debuffObject = transform.GetChild(1);
            _debuffImage = debuffObject.GetComponentInChildren(typeof(Image), true) as Image;
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
        public class RarityIdentifierSprites
        {
            [SerializeField] private Sprite _common;
            [SerializeField] private Sprite _rare;
            [SerializeField] private Sprite _epic;

            public Sprite this[Rarity rarity] => rarity switch
            {
                Rarity.COMMON=>_common,
                Rarity.RARE=>_rare,
                Rarity.EPIC=>_epic,
                _ => throw new ArgumentOutOfRangeException(nameof(rarity), rarity, null)
            };
        }
        
        #endregion
    }
}