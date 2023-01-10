using System;
using UnityEngine;

namespace Cards.CardElementClasses
{
    [Serializable]
    public class FixedCardElementClass
    {
        [field: SerializeField] public Rarity Rarity { get; private set; }
        [field: SerializeField] public float[] Parameters { get; private set; }
        [field: SerializeField] public CardElementClassAttributes Attributes { get; private set; }

        public void ToInt()
        {
            for (int i = 0; i < Parameters.Length; i++)
            {
                Parameters[i] = (int) Parameters[i];
            }
        }
    }
}