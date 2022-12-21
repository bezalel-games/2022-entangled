using Cards;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class CardSelectionUI : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private GameObject _firstSelectedCard;
        [SerializeField] private CardUI _leftCard;
        [SerializeField] private CardUI _rightCard;

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

        public void ShowCards(Card leftCard, Card rightCard)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(_firstSelectedCard);
            _cardsParent.SetActive(true);
            _leftCard.Card = leftCard;
            _rightCard.Card = rightCard;
        }

        #endregion
    }
}