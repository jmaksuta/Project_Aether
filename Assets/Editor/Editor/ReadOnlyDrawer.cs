using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor
{

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Ensures the property takes up the correct amount of space
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Save the current GUI enabled state
            var previousGUIState = GUI.enabled;

            // Disable GUI for this property
            GUI.enabled = false;

            // Draw the property field normally
            EditorGUI.PropertyField(position, property, label);

            // Restore the previous GUI enabled state
            GUI.enabled = previousGUIState;
        }
    }
}
