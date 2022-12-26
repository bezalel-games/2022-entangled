using UnityEngine;

namespace Cards.CardElementClasses
{
    [CreateAssetMenu(fileName = "Card Element Class Attributes",
        menuName = "Entangled/Cards/Card Element Class Attributes")]
    public class CardElementClassAttributes : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }

        [field: SerializeField] [field: TextArea(1, 3)] public string Description { get; private set; }

        [field: SerializeField] public Sprite CardSprite { get; private set; }
    }
}