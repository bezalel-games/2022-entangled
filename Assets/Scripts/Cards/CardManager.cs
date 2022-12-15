﻿using System;
using Cards.Buffs.ActiveBuffs;
using Cards.Buffs.PassiveBuffs;
using Cards.Debuffs;
using Managers;
using UnityEngine;

namespace Cards
{
    public class CardManager : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] [TextArea(5, 20)] private string _cardFormat;

        #endregion

        #region Non-Serialized Fields

        private readonly Card _leftCard = new Card(new EnlargeYoyo(1.3f), new MoreEnemies(0, 1));
        private readonly Card _rightCard = new Card(new SwapPositionWithYoyo(), new MoreEnemies(0, 1));

        #endregion

        #region Events

        public Action<string, string> ActivateCardSelection;

        #endregion

        #region Public Methods

        public void ShowCards()
        {
            ActivateCardSelection.Invoke(_leftCard.ToString(_cardFormat), _rightCard.ToString(_cardFormat));
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