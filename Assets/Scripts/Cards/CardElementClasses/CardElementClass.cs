using System;
using UnityEngine;
using static Cards.Rarity;

namespace Cards.CardElementClasses
{
    public abstract class CardElementClass<T> : ScriptableObject
    {
        [field: SerializeField] public CardElementClassAttributes Attributes { get; private set; }

        [SerializeField] private T _commonVariant;
        [SerializeField] private T _rareVariant;
        [SerializeField] private T _epicVariant;

        public T this[Rarity rarity] => rarity switch
        {
            COMMON => _commonVariant,
            RARE => _rareVariant,
            EPIC => _epicVariant,
            _ => throw new ArgumentOutOfRangeException(nameof(rarity), rarity, null)
        };
    }

    [Serializable]
    public class CardElementClassAttributes
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] [field: TextArea(1, 3)]  public string Description { get; private set; }
    }
}