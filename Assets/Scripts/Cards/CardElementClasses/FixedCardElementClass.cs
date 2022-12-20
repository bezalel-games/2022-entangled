using System;
using UnityEngine;
using UnityEngine.Serialization;
using static Cards.Rarity;

namespace Cards.CardElementClasses
{
    [Serializable]
    public class FixedCardElementClass
    {
        [SerializeField] private Rarity _rarity;
        [SerializeField] private float _parameter;
        [field: SerializeField] public CardElementClassAttributes Attributes { get; private set; }

        public void ToInt()
        {
            _parameter = (int)_parameter;
        }
    }
}