using System;
using System.Collections.Generic;
using System.Linq;
using Cards.Factory;
using UI;
using UnityEngine;
using Utils.SaveUtils;

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

        // private PlayerDeck _playerDeck;
        // private RunDeck _runDeck;
        private CardPool _cardPool;
        private Card _leftCard;
        private Card _rightCard;
        private Action _finishedChoosingCardsAction;

        #endregion

        #region Function Events

        private void Awake()
        {
            InitPool();
        }

        // private void OnDestroy()
        // {
        //     SaveSystem.SaveData(new() { { SaveSystem.DataType.DECK, _playerDeck } });
        // }

        #endregion

        #region Public Methods

        public void ShowCards(Action finishedChoosingCardsAction)
        {
            // bool leftIsDeckCard = true;
            _finishedChoosingCardsAction = finishedChoosingCardsAction;
            _leftCard = GenerateNewCard();
            
            // _leftCard = _runDeck.Draw();
            // if (_leftCard == null)
            // {
            //     _leftCard = GenerateNewCard();
            //     leftIsDeckCard = false;
            // }

            _rightCard = GenerateNewCard();
            _ui.ShowCards(_leftCard, _rightCard, false);
        }

        public void ChooseLeftCard() => ChooseCard(_leftCard);

        public void ChooseRightCard() => ChooseCard(_rightCard);

        #endregion

        #region Private Methods

        private void InitPool()
        {
            _cardPool = new CardPool(_commonWeight, _rareWeight, _epicWeight);
            var allRarities = Rarities.All;
            _cardPool.Add(DebuffType.MORE_GOOMBAS, allRarities);
            _cardPool.Add(DebuffType.MORE_SHOOTERS, allRarities);
            _cardPool.Add(DebuffType.MORE_FUMERS, allRarities);
            _cardPool.Add(DebuffType.TOUGHER_GOOMBAS, allRarities);
            _cardPool.Add(DebuffType.TOUGHER_SHOOTERS, allRarities);
            _cardPool.Add(DebuffType.TOUGHER_FUMERS, allRarities);
            _cardPool.Add(DebuffType.FASTER_GOOMBAS, allRarities);
            _cardPool.Add(DebuffType.FASTER_SHOOTERS, allRarities);
            _cardPool.Add(DebuffType.FASTER_FUMERS, allRarities);
            
            _cardPool.Add(BuffType.EXPLOSIVE_YOYO, allRarities);
            _cardPool.Add(BuffType.ENLARGE_YOYO, allRarities);
            _cardPool.Add(BuffType.SWAP_POSITIONS_WITH_YOYO, _factory.SwapPositionWithYoyo.Rarity);
        }
        
        // private void InitDecks()
        // {
        //     _playerDeck = new PlayerDeck(_factory);
        //
        //     if (SaveSystem.DataSaved(_playerDeck))
        //         SaveSystem.LoadData(new() { { SaveSystem.DataType.DECK, _playerDeck } });
        //     else
        //         _playerDeck.InitEmpty();
        //     _runDeck = new RunDeck(_playerDeck, _cardPool);
        // }
        
        private Card GenerateNewCard()
        {
            var buff = _cardPool.GetRandomBuff();
            var debuff = _cardPool.GetRandomDebuff();
            return _factory.Create(buff.type, buff.rarity, debuff.type, debuff.rarity);
        }

        private void ChooseCard(Card card)
        {
            card.Apply(_cardPool);
            // _runDeck.ReplaceCard(card);
            _finishedChoosingCardsAction.Invoke();
        }

        #endregion

        #region Classes

        public class PlayerDeck : ISavable<List<Card.CardEssence>>
        {
            private readonly CardFactory _factory;
            private List<Card> _deck;

            public PlayerDeck(CardFactory factory)
            {
                _factory = factory;
                _deck = null;
            }


            // To allow a deck to be passed as a list
            public static implicit operator List<Card>(PlayerDeck deck) => deck._deck;

            public void InitEmpty() => _deck = new List<Card>();

            public SaveSystem.DataType GetDataType()
            {
                return SaveSystem.DataType.DECK;
            }

            List<Card.CardEssence> ISavable<List<Card.CardEssence>>.ToSave()
            {
                var serializableCards = new List<Card.CardEssence>(_deck.Count);
                serializableCards.AddRange(_deck.Select(card => card.Essence));
                return serializableCards;
            }

            public void FromLoad(List<Card.CardEssence> data)
            {
                _deck = new List<Card>(data.Count);
                _deck.AddRange(data.Select(cardEssence => _factory.Create(cardEssence.Buff.type,
                    cardEssence.Buff.rarity, cardEssence.Debuff.type, cardEssence.Debuff.rarity)));
            }
        }

        #endregion
    }
}