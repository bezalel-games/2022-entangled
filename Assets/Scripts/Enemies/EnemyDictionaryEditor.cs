using UnityEditor;
using UnityEngine.UIElements;

namespace Enemies
{
    // [CustomEditor(typeof(EnemyDictionary))]
    public class EnemyDictionaryEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement inspector = new();
            inspector.Add(new Label("This is a custom inspector"));
            return inspector;
        }
    }
}