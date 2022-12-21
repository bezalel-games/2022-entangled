using System;
using UI;
using UnityEngine;
using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(CardUI))]
    public class CardUIEditor : UnityEditor.Editor
    {
        private SerializedProperty _titleFormat;
        private SerializedProperty _buffFormat;
        private SerializedProperty _debuffFormat;
        private SerializedProperty _rarityColors;

        private void OnEnable()
        {
            _titleFormat = serializedObject.FindProperty("_titleFormat");
            _buffFormat = serializedObject.FindProperty("_buffFormat");
            _debuffFormat = serializedObject.FindProperty("_debuffFormat");
            _rarityColors = serializedObject.FindProperty("_rarityColors");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            GUILayout.Label("Card Title Format:");
            _titleFormat.stringValue = GUILayout.TextArea(_titleFormat.stringValue);
            GUILayout.Label("where  {0} = Buff part  {1} = Debuff part\n");
            
            GUILayout.Label("Card Buff Format:");
            _buffFormat.stringValue = GUILayout.TextArea(_buffFormat.stringValue);
            GUILayout.Label("where  {0} = Name  {1} = Description  {2} = Rarity\n");
            
            GUILayout.Label("Card Debuff Format:");
            _debuffFormat.stringValue = GUILayout.TextArea(_debuffFormat.stringValue);
            GUILayout.Label("where  {0} = Name  {1} = Description  {2} = Rarity\n");

            EditorGUILayout.PropertyField(_rarityColors);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}