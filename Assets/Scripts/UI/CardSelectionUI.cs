using Cards;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class CardSelectionUI : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private GameObject _firstSelectedCard;
        [SerializeField] private TextMeshProUGUI _leftCard;
        [SerializeField] private TextMeshProUGUI _rightCard;
        [SerializeField] [TextArea(5, 20)] private string _cardFormat;

        #endregion

        #region Non-Serialized Fields

        private GameObject _cardsParent;

        #endregion

        #region Function Events

        private void Awake()
        {
            _cardsParent = transform.GetChild(0).gameObject;
        }

        #endregion

        #region Public Methods

        public void HideCardSelection()
        {
            _cardsParent.SetActive(false);
        }

        public void ShowCards(Card leftCardText, Card rightCardText)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(_firstSelectedCard);
            _cardsParent.SetActive(true);
            _leftCard.text = leftCardText.ToString(_cardFormat);
            _rightCard.text = rightCardText.ToString(_cardFormat);
        }

        #endregion
    }
}