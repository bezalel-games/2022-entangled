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
        [SerializeField] private GlowColor _buffGlowColor;

        #endregion

        #region Non-Serialized Fields

        private CardPool _cardPool;
        private Card _leftCard;
        private Card _rightCard;
        private Action _finishedChoosingCardsAction;

        #endregion

        #region Properties

        public static GlowColor BuffGlowColor { get; private set; }

        #endregion

        #region Events

        public static event Action<Card, Card, bool> StartedChoosingCards;
        public static event Action FinishedChoosingCards;
        
        #endregion

        #region Function Events

        private void Awake()
        {
            InitPool();
            BuffGlowColor = _buffGlowColor;
        }

        #endregion

        #region Public Methods

        public void ShowCards(Action finishedChoosingCardsAction)
        {
            _finishedChoosingCardsAction = finishedChoosingCardsAction;
            _leftCard = GenerateNewCard();
            _rightCard = GenerateNewCard();
            StartedChoosingCards?.Invoke(_leftCard, _rightCard, false);
        }

        public void ChooseLeftCard() => ChooseCard(_leftCard);

        public void ChooseRightCard() => ChooseCard(_rightCard);

        #endregion

        #region Private Methods

        private void InitPool()
        {
            _cardPool = new CardPool(_commonWeight, _rareWeight, _epicWeight);
            var allRarities = new Rarities(Rarity.EPIC); //Rarities.All);
            _cardPool.Add(DebuffType.MORE_GOOMBAS, allRarities);
            _cardPool.Add(DebuffType.MORE_SHOOTERS, allRarities);
            _cardPool.Add(DebuffType.MORE_FUMERS, allRarities);
            _cardPool.Add(DebuffType.TOUGHER_GOOMBAS, allRarities);
            _cardPool.Add(DebuffType.TOUGHER_SHOOTERS, allRarities);
            _cardPool.Add(DebuffType.TOUGHER_FUMERS, allRarities);
            _cardPool.Add(DebuffType.FASTER_GOOMBAS, allRarities);
            _cardPool.Add(DebuffType.FASTER_SHOOTERS, allRarities);
            _cardPool.Add(DebuffType.FASTER_FUMERS, allRarities);
            _cardPool.Add(DebuffType.DECREASE_DAMAGE, allRarities);
            _cardPool.Add(DebuffType.DECREASE_SHOT_DISTANCE, allRarities);
            _cardPool.Add(DebuffType.DECREASE_MP_REGEN, allRarities);
            
            _cardPool.Add(DebuffType.HOMING_SHOTS, _factory.HomingProjectiles.Rarity);
            _cardPool.Add(DebuffType.SPLIT_FUMERS, _factory.SplittingFumers.Rarity);
            _cardPool.Add(DebuffType.STUNNING_GOOMBAS, _factory.StunningGoombas.Rarity);
            
            // _cardPool.Add(DebuffType.SPLIT_GOOMBAS, allRarities);
            // _cardPool.Add(DebuffType.SPLIT_SHOOTERS, allRarities);
            
            _cardPool.Add(BuffType.ENLARGE_YOYO, allRarities);

            _cardPool.Add(BuffType.EXPLOSIVE_YOYO, _factory.ExplosiveYoyo.Rarity);
            _cardPool.Add(BuffType.SWAP_POSITIONS_WITH_YOYO, _factory.SwapPositionWithYoyo.Rarity);
            _cardPool.Add(BuffType.LEAVE_TRAIL, _factory.LeaveTrail.Rarity);
        }
        
        private Card GenerateNewCard()
        {
            var buff = _cardPool.GetRandomBuff();
            var debuff = _cardPool.GetRandomDebuff();
            return _factory.Create(buff.type, buff.rarity, debuff.type, debuff.rarity);
        }

        private void ChooseCard(Card card)
        {
            FinishedChoosingCards?.Invoke();
            card.Apply(_cardPool);
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
        
        [Serializable]
        public class GlowColor
        {
            [SerializeField][HideInInspector] private Color _superDebuff;
            [SerializeField][HideInInspector] private Color _debuff;
            [SerializeField][HideInInspector] private Color _neutral;
            [SerializeField][HideInInspector] private Color _buff;
            [SerializeField] private Color _superBuff;

            public Color this[Rarity buff, Rarity debuff]
            {
                get
                {
                    int value = ((int) buff - (int) debuff);
                    return value switch
                    {
                        // -2 => _superDebuff,
                        // -1 => _debuff,
                        // 0 => _neutral,
                        // 1 => _buff,
                        // 2 => _superBuff,
                        _ => _superBuff
                    };
                }
            }
        }
    }
}