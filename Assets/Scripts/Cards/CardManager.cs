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

        #endregion

        #region Public Methods

        public void ShowCards()
        {
            _ui.ShowCards(_leftCard.ToString(_cardFormat), _rightCard.ToString(_cardFormat));
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