using UnityEditor;
using UnityEngine;

namespace CatCode.Editor
{
    [CustomPropertyDrawer(typeof(InspectorReadOnlyAttribute))]
    public class InspectorReadOnlyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var readOnlyAttribute = (InspectorReadOnlyAttribute)attribute;

            var readOnlyInPlay = HasFlag(readOnlyAttribute.Mode, InspectorReadOnlyMode.PlayMode) && Application.isPlaying;
            var readOnlyInEdit = HasFlag(readOnlyAttribute.Mode, InspectorReadOnlyMode.EditMode) && !Application.isPlaying;
            bool isReadOnly = readOnlyInPlay || readOnlyInEdit;

            GUI.enabled = !isReadOnly;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;

        }

        private bool HasFlag(InspectorReadOnlyMode mask, InspectorReadOnlyMode value)
            => (mask & value) == value;

    }
}