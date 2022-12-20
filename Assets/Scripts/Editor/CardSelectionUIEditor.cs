using UI;
using UnityEngine;
using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(CardSelectionUI))]
    public class CardSelectionUIEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Label("{0}:  Buff name\n{1}:  Buff description\n{2}:  Buff rarity\n" +
                            "{3}:  Debuff name\n{4}:  Debuff description\n{5}:  Debuff rarity\n");
        }
    }
}