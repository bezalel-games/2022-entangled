using UI;
using UnityEngine;
using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(CardUI))]
    public class CardUIEditor : UnityEditor.Editor
    {
        private SerializedProperty _buffFormat;
        private SerializedProperty _debuffFormat;
        private SerializedProperty _rarityIdentifierSprites;
        private SerializedProperty _glowColor;

        private void OnEnable()
        {
            _buffFormat = serializedObject.FindProperty("_buffFormat");
            _debuffFormat = serializedObject.FindProperty("_debuffFormat");
            _rarityIdentifierSprites = serializedObject.FindProperty("_rarityIdentifierSprites");
            _glowColor = serializedObject.FindProperty("_glowColor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            GUILayout.Label("Card Buff Format:");
            _buffFormat.stringValue = GUILayout.TextArea(_buffFormat.stringValue);
            GUILayout.Label("where  {0} = Description  {1} = Rarity\n");
            
            GUILayout.Label("Card Debuff Format:");
            _debuffFormat.stringValue = GUILayout.TextArea(_debuffFormat.stringValue);
            GUILayout.Label("where  {0} = Description  {1} = Rarity\n");

            EditorGUILayout.PropertyField(_rarityIdentifierSprites);
            EditorGUILayout.PropertyField(_glowColor);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}