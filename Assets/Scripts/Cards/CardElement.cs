using Cards.CardElementClasses;
using Cards.Factory;
using UnityEngine;

namespace Cards
{
    public abstract class CardElement
    {
        public static readonly string VariablePlaceholder = "VAR_TXT"; 
        
        private readonly CardElementClassAttributes _attributes;
        
        public string Description
        {
            get
            {
                string s = _attributes.Description;
                string replacement = Rarity switch
                {
                    Rarity.COMMON => _attributes.CommonText,
                    Rarity.RARE => _attributes.RareText,
                    Rarity.EPIC => _attributes.EpicText,
                };
                
                Debug.Log($"{Rarity}: {replacement}");

                s = s.Replace(VariablePlaceholder, replacement);
                s = s.Trim();
                return char.ToUpper(s[0]) + s.Substring(1);
            }
        }
        
        public Sprite CardSprite => _attributes.CardSprite;
        public Rarity Rarity { get; }

        protected CardElement(CardElementClassAttributes attributes, Rarity rarity)
        {
            _attributes = attributes;
            Rarity = rarity;
        }

        public virtual void UpdatePool(CardPool pool)
        {
            var allRarities = Rarities.All;
            foreach (var buff in _attributes.UnlockedBuffs)
                pool.Add(buff, allRarities);
            foreach (var debuff in _attributes.UnlockedDebuffs)
                pool.Add(debuff, allRarities);
            // if (_attributes.SingleUse)
            RemoveSelfFromPool(pool);
        }

        protected abstract void RemoveSelfFromPool(CardPool pool);
    }
}