using Cards.Factory;
using UnityEngine;

namespace Cards.CardElementClasses
{
    [CreateAssetMenu(fileName = "Card Element Class Attributes",
        menuName = "Entangled/Cards/Card Element Class Attributes")]
    public class CardElementClassAttributes : ScriptableObject
    {
        [field: SerializeField] [field: TextArea(1, 3)] 
        [field: Tooltip("Add \"VAR_TXT\" to description to be replaced with variables text")]
        public string Description { get; private set; }

        [field: SerializeField] public Sprite CardSprite { get; private set; }

        [field: SerializeField] public BuffType[] UnlockedBuffs { get; private set; }

        [field: SerializeField] public DebuffType[] UnlockedDebuffs { get; private set; }

        [field: SerializeField] public bool SingleUse { get; private set; }

        [field: SerializeField] public string CommonText { get; private set; } = "Slightly";
        [field: SerializeField] public string RareText { get; private set; } = "";
        [field: SerializeField] public string EpicText { get; private set; } = "Greatly";
    }
}