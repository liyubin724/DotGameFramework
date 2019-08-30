using Dot.Core.UI;
using UnityEditor;
using UnityEditor.UI;

namespace DotEditor.Core.UI
{
    [CustomEditor(typeof(DynamicAtlasImage), false)]
    public class DynamicAtlasImageEditor : ImageEditor
    {
        private SerializedProperty m_AtlasName;
        private SerializedProperty m_RawImagePath;
        private SerializedProperty m_IsSetNativeSize;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_AtlasName = serializedObject.FindProperty("m_AtlasName");
            m_RawImagePath = serializedObject.FindProperty("m_RawImagePath");
            m_IsSetNativeSize = serializedObject.FindProperty("m_IsSetNativeSize");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_AtlasName);
            EditorGUILayout.PropertyField(m_RawImagePath);
            EditorGUILayout.PropertyField(m_IsSetNativeSize);

            AppearanceControlsGUI();
            RaycastControlsGUI();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
