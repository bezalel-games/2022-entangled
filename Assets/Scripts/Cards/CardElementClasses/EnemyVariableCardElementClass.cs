using System;
using UnityEngine;

namespace Cards.CardElementClasses
{
    [Serializable]
    public class EnemyVariableCardElementClass : VariableCardElementClass
    {
        [field: SerializeField] [field: Tooltip("The enemy's index in the dictionary")]
        public int EnemyIndex { get; private set; }
    }
}