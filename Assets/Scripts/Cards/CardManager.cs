using System;
using Cards.Buffs.ActiveBuffs;
using Cards.Buffs.PassiveBuffs;
using Cards.Debuffs;
using UI;
using UnityEngine;

namespace Cards
{
    public class CardManager : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private CardSelectionUI _ui;

        #endregion

        #region Non-Serialized Fields

        private Card _leftCard;
        private Card _rightCard;
        private Action _finishedChoosingCardsAction;

        #endregion

        #region Public Methods

        public void ShowCards(Action finishedChoosingCardsAction)
        {
            _finishedChoosingCardsAction = finishedChoosingCardsAction;
            _ui.ShowCards(_leftCard, _rightCard);
        }

        public void ChooseLeftCard() => ChooseCard(_leftCard);

        public void ChooseRightCard() => ChooseCard(_rightCard);

        #endregion

        #region Private Methods

        private void ChooseCard(Card card)
        {
            card.Apply();
            _finishedChoosingCardsAction.Invoke();
        }

        #endregion
    }
}