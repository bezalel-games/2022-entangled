using System;
using Cards.Buffs.ActiveBuffs;
using Cards.Buffs.PassiveBuffs;
using Cards.Debuffs;
using Managers;
using UI;
using UnityEngine;

namespace Cards
{
    public class CardManager : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] [TextArea(5, 20)] private string _cardFormat;
        [SerializeField] private CardSelectionUI _ui;

        #endregion

        #region Non-Serialized Fields

        private readonly Card _leftCard = new Card(new EnlargeYoyo(1.3f), new MoreEnemies(0, 3));
        private readonly Card _rightCard = new Card(new SwapPositionWithYoyo(), new TougherEnemies(0, 1));
        private Action _finishedChoosingCardsAction;

        #endregion

        #region Public Methods

        public void ShowCards(Action finishedChoosingCardsAction)
        {
            _finishedChoosingCardsAction = finishedChoosingCardsAction;
            _ui.ShowCards(_leftCard.ToString(_cardFormat), _rightCard.ToString(_cardFormat));
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