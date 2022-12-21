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
        [field: SerializeField] public float Parameter { get; private set; }
        [field: SerializeField] public CardElementClassAttributes Attributes { get; private set; }

        public void ToInt()
        {
            Parameter = (int)Parameter;
        }
    }
}