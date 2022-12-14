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
        [SerializeField] private CardManager _cardManager;
        [SerializeField] private TextMeshProUGUI _leftCard;
        [SerializeField] private TextMeshProUGUI _rightCard;

        #endregion
        
        #region Non-Serialized Fields

        private GameObject _cardsParent;
        
        #endregion

        #region Function Events

        private void Awake()
        {
            // _cardManager.ActivateCardSelection = ShowCards;
            _cardsParent = transform.GetChild(0).gameObject;
        }

        private void Start()
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(_firstSelectedCard);
        }

        private void OnDestroy()
        {
            // _cardManager.ActivateCardSelection -= ShowCards;
        }

        #endregion

        #region Public Methods

        public void HideCardSelection()
        {
            _cardsParent.SetActive(false);
        }

        #endregion

        #region Private Methods

        public void ShowCards(string leftCardText, string rightCardText)
        {
            print("showing");
            _cardsParent.SetActive(true);
            _leftCard.text = leftCardText;
            _rightCard.text = rightCardText;
        }

        #endregion
    }
}