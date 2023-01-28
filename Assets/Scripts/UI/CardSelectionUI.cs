using Cards;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class CardSelectionUI : MonoBehaviourExt
    {
        #region Serialized Fields

        [SerializeField] private GameObject _firstSelectedCard;
        [SerializeField] private CardUI _leftCard;
        [SerializeField] private CardUI _rightCard;
        [SerializeField] private GameObject _leftIsDeckCard;
        [SerializeField]private float _hideAnimationDuration = 0.5f;
        [SerializeField]private float _showAnimationDuration = 0.5f;

        #endregion

        #region Non-Serialized Fields

        private GameObject _cardsParent;

        #endregion

        #region Function Events

        private void Awake()
        {
            _cardsParent = transform.GetChild(0).gameObject;
            CardManager.StartedChoosingCards += ShowCards;
        }

        private void OnDestroy()
        {
            CardManager.StartedChoosingCards -= ShowCards;
        }

        #endregion

        #region Public Methods

        public void HideCardSelection()
        {
            _leftCard.Hide(_hideAnimationDuration);
            _rightCard.Hide(_hideAnimationDuration, true);
            DelayInvoke(()=>_cardsParent.SetActive(false), _hideAnimationDuration);
        }
        
        #endregion
        
        #region Private Methods

        private void ShowCards(Card leftCard, Card rightCard, bool leftIsDeckCard)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(_firstSelectedCard);
            _cardsParent.SetActive(true);
            _leftCard.Card = leftCard;
            _rightCard.Card = rightCard;
            _leftCard.Show(_showAnimationDuration);
            _rightCard.Show(_showAnimationDuration, true);
            _leftIsDeckCard.SetActive(leftIsDeckCard);
        }

        #endregion
    }
}