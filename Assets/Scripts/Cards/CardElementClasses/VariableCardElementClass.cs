using System;
using UnityEngine;
using static Cards.Rarity;

namespace Cards.CardElementClasses
{
    [Serializable]
    public class VariableCardElementClass
    {
        [SerializeField][HideInInspector] private float _commonVariant;
        [SerializeField][HideInInspector] private float _rareVariant;
        [SerializeField] private float _epicVariant;
        [field: SerializeField] public CardElementClassAttributes Attributes { get; private set; }

        public float this[Rarity rarity] => rarity switch
        {
            COMMON => _commonVariant,
            RARE => _rareVariant,
            EPIC => _epicVariant,
            _ => throw new ArgumentOutOfRangeException(nameof(rarity), rarity, null)
        };

        public void ToInt()
        {
            _commonVariant = (int)_commonVariant;
            _rareVariant = (int)_rareVariant;
            _epicVariant = (int)_epicVariant;
        }
    }
}