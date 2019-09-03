using UnityEditor;

namespace DotEditor.Core.Asset
{
    [CustomEditor(typeof(BaseActionSchema))]
    public class BaseActionSchemaEditor : Editor
    {
        private SerializedProperty isEnable;
        private SerializedProperty actionName;

        protected virtual void OnEnable()
        {
            isEnable = serializedObject.FindProperty("isEnable");
            actionName = serializedObject.FindProperty("actionName");
        }

        protected void DrawIsEnable()
        {
            EditorGUILayout.PropertyField(isEnable);
        }

        protected void DrawActionName()
        {
            EditorGUILayout.PropertyField(actionName);
        }
    }
}
