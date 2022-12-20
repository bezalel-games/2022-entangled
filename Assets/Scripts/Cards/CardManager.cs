using System;
using System.Linq;
using Cards.Buffs.ActiveBuffs;
using Cards.Buffs.PassiveBuffs;
using Cards.Debuffs;
using Cards.Factory;
using UI;
using UnityEngine;

namespace Cards
{
    public class CardManager : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private CardSelectionUI _ui;
        [SerializeField] private CardFactory _factory;
        [SerializeField] private int _commonWeight;
        [SerializeField] private int _rareWeight;
        [SerializeField] private int _epicWeight;

        #endregion

        #region Non-Serialized Fields

        private CardPool _cardPool;
        private Card _leftCard;
        private Card _rightCard;
        private Action _finishedChoosingCardsAction;

        #endregion

        #region Function Events

        public void Awake()
        {
            _cardPool = new CardPool(new[] { _commonWeight, _rareWeight, _epicWeight });
            foreach (var rarity in Enum.GetValues(typeof(Rarity)).Cast<Rarity>())
            {
                _cardPool.Add(BuffType.EXPLOSIVE_YOYO, rarity);
                _cardPool.Add(BuffType.ENLARGE_YOYO, rarity);
                _cardPool.Add(DebuffType.MORE_GOOMBAS, rarity);
                _cardPool.Add(DebuffType.MORE_SHOOTERS, rarity);
                _cardPool.Add(DebuffType.TOUGHER_GOOMBAS, rarity);
                _cardPool.Add(DebuffType.TOUGHER_SHOOTERS, rarity);
            }

            _cardPool.Add(BuffType.ENLARGE_YOYO, _factory.SwapPositionWithYoyo.Rarity);
        }

        #endregion

        #region Public Methods

        public void ShowCards(Action finishedChoosingCardsAction)
        {
            _finishedChoosingCardsAction = finishedChoosingCardsAction;
            GenerateNewCards();
            _ui.ShowCards(_leftCard, _rightCard);
        }

        public void ChooseLeftCard() => ChooseCard(_leftCard);

        public void ChooseRightCard() => ChooseCard(_rightCard);

        #endregion

        #region Private Methods

        private void GenerateNewCards()
        {
            var buff = _cardPool.GetRandomBuff();
            var debuff = _cardPool.GetRandomDebuff();
            _leftCard = _factory.Create(buff.type, buff.rarity, debuff.type, debuff.rarity);
            buff = _cardPool.GetRandomBuff();
            debuff = _cardPool.GetRandomDebuff();
            _rightCard = _factory.Create(buff.type, buff.rarity, debuff.type, debuff.rarity);
        }

        private void ChooseCard(Card card)
        {
            card.Apply();
            _finishedChoosingCardsAction.Invoke();
        }

        #endregion
    }
}