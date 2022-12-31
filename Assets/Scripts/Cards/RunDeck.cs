using System.Collections.Generic;
using Cards.Factory;
using Random = UnityEngine.Random;

namespace Cards
{
    public class RunDeck
    {
        #region Fields

        private readonly List<Card> _playerDeck;
        private readonly List<(int deckIndex, Card card)> _cards;
        private readonly List<(int deckIndex, Card card)> _unavailableCards = new();
        private int _cardsIndexOfNextCard;
        private int _deckIndexOfDrawnCard = -1;
        private readonly CardPool _cardPool;

        #endregion

        #region Properties

        private int NextCardIndex
        {
            get
            {
                var next = _cardsIndexOfNextCard;
                _cardsIndexOfNextCard--;
                if (_cardsIndexOfNextCard < 0)
                    _cardsIndexOfNextCard += _cards.Count;
                return next;
            }
        }

        #endregion

        #region Constructors

        public RunDeck(List<Card> playerDeck, CardPool cardPool)
        {
            _playerDeck = playerDeck;
            _cards = new();
            _cardPool = cardPool;
            cardPool.PoolUpdated += ValidateCards;
            for (int i = 0; i < playerDeck.Count; ++i)
            {
                var card = playerDeck[i];
                var containsBuff = cardPool.Contains(card.BuffType);
                var containsDebuff = cardPool.Contains(card.DebuffType);
                if (containsBuff && containsDebuff)
                {
                    _cards.Add((i, playerDeck[i]));
                    continue;
                }

                _unavailableCards.Add((i, playerDeck[i]));
            }

            Shuffle(_cards);
            _cardsIndexOfNextCard = _cards.Count - 1;
        }

        #endregion

        #region Public Methods

        public Card Draw()
        {
            if (_cards.Count == 0)
            {
                _deckIndexOfDrawnCard = _cards.Count;
                return null;
            }

            var cardToDrawIndex = NextCardIndex;
            var cardsEntry = _cards[cardToDrawIndex];
            _cards.RemoveAt(cardToDrawIndex);
            _deckIndexOfDrawnCard = cardsEntry.deckIndex;
            return cardsEntry.card;
        }

        public void ReplaceCard(Card newCard)
        {
            if (_deckIndexOfDrawnCard == _cards.Count)
                _playerDeck.Add(newCard);
            else
                _playerDeck[_deckIndexOfDrawnCard] = newCard;
            _deckIndexOfDrawnCard = -1;
        }

        #endregion

        #region Private Methods

        private void ValidateCards()
        {
            _cards.RemoveAll(element => !_cardPool.Contains(element.card.BuffType) ||
                                        !_cardPool.Contains(element.card.DebuffType));
            for (int i = _unavailableCards.Count - 1; i >= 0; --i)
            {
                var cardsElement = _unavailableCards[i];
                if (!_cardPool.Contains(cardsElement.card.BuffType) ||
                    !_cardPool.Contains(cardsElement.card.DebuffType))
                    return;
                Insert(cardsElement);
                _unavailableCards.RemoveAt(i);
            }
        }

        private void Insert((int index, Card newCard) cardsElement)
        {
            int k = Random.Range(0, _cards.Count);
            _cards.Add(_cards[k]);
            _cards[k] = cardsElement;
        }

        private static void Shuffle<T>(IList<T> deck)
        {
            for (int n = deck.Count; n > 1;)
            {
                int k = Random.Range(0, n--);
                (deck[k], deck[n]) = (deck[n], deck[k]);
            }
        }

        #endregion
    }
}