using Cards;
using Cards.Factory;
using UnityEngine;
using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(CardFactory))]
    public class CardFactoryEditor : UnityEditor.Editor
    {
        private SerializedProperty _buffToApply;
        private SerializedProperty _debuffToApply;
        private SerializedProperty _buffRarity;
        private SerializedProperty _debuffRarity;

        private void OnEnable()
        {
            _buffToApply = serializedObject.FindProperty("_buffToApply");
            _buffRarity = serializedObject.FindProperty("_buffRarity");
            _debuffToApply = serializedObject.FindProperty("_debuffToApply");
            _debuffRarity = serializedObject.FindProperty("_debuffRarity");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var factory = (CardFactory)target;
            serializedObject.Update();

            if (GUILayout.Button("Apply Buff and Debuff"))
                factory.Create((BuffType)_buffToApply.intValue, (Rarity)_buffRarity.intValue, (DebuffType)_debuffToApply.intValue,
                    (Rarity)_debuffRarity.intValue).Apply(null);
        }
    }
}