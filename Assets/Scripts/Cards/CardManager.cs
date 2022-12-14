using System;
using Cards.Buffs.ActiveBuffs;
using Cards.Buffs.PassiveBuffs;
using Managers;
using UnityEngine;

namespace Cards
{
    [CreateAssetMenu(fileName = "Card Manager Asset", menuName = "Entangled/Card Manager Asset", order = 0)]
    public class CardManager : ScriptableObject
    {
        #region Serialized Fields

        [SerializeField] [TextArea(5, 20)] private string _cardFormat;

        #endregion

        #region Non-Serialized Fields

        private readonly Card _leftCard = new Card(new EnlargeYoyo(1.3f), null);
        private readonly Card _rightCard = new Card(new SwapPositionWithYoyo(), null);

        #endregion

        #region Events

        public event Action<string, string> ActivateCardSelection;

        #endregion

        #region Public Methods

        public void ShowCards()
        {
            ActivateCardSelection?.Invoke(_leftCard.ToString(_cardFormat), _rightCard.ToString(_cardFormat));
        }

        public void ChooseLeftCard()
        {
            _leftCard.Apply();
            GameManager.CardChosen();
        }

        public void ChooseRightCard()
        {
            _rightCard.Apply();
            GameManager.CardChosen();
        }

        #endregion
    }
}