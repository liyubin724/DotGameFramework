using UnityEditor;

namespace DotEditor.Core.Asset
{
    [CustomEditor(typeof(AssetBundleGroupSchema))]
    public class AssetBundleGroupSchemaEditor : BaseGroupSchemaEditor
    {
        private SerializedProperty isMain;

        protected override void OnEnable()
        {
            base.OnEnable();
            isMain = serializedObject.FindProperty("isMain");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawIsEnable();
            DrawGroupInfo();
            EditorGUILayout.PropertyField(isMain);
            EditorGUILayout.Space();
            DrawFilters();
            EditorGUILayout.Space();
            DrawActions();
            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
