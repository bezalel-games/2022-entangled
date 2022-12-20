using System;
using UnityEngine;
using UnityEngine.Serialization;
using static Cards.Rarity;

namespace Cards.CardElementClasses
{
    [Serializable]
    public class FixedCardElementClass
    {
        [field: SerializeField] public Rarity Rarity { get; private set; }
        [SerializeField] private float _parameter;
        [field: SerializeField] public CardElementClassAttributes Attributes { get; private set; }

        public void ToInt()
        {
            _parameter = (int)_parameter;
        }
    }
}