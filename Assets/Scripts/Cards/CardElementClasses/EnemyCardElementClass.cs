using System.ComponentModel;
using UnityEngine;

namespace Cards.CardElementClasses
{
    [CreateAssetMenu(fileName = "Float Card Element Class", menuName = "Entangled/Cards/Float Card Element Class")]
    public abstract class EnemyCardElementClass<T> : CardElementClass<T>
    {
        [field: SerializeField] [field: Space(20)] [Description("The enemy's index in the dictionary")]
        public int EnemyIndex { get; private set; }
    }
}