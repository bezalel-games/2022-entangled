using Cards.Factory;
using UnityEngine;

namespace Cards.CardElementClasses
{
    [CreateAssetMenu(fileName = "Card Element Class Attributes",
        menuName = "Entangled/Cards/Card Element Class Attributes")]
    public class CardElementClassAttributes : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string TitlePart { get; private set; }

        [field: SerializeField] [field: TextArea(1, 3)]
        public string Description { get; private set; }

        [field: SerializeField] public Sprite CardSprite { get; private set; }

        [field: SerializeField] public BuffType[] UnlockedBuffs { get; private set; }

        [field: SerializeField] public DebuffType[] UnlockedDebuffs { get; private set; }

        [field: SerializeField] public bool SingleUse { get; private set; }
    }
}