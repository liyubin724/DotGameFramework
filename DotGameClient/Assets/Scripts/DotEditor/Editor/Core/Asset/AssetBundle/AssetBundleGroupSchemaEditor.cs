using UnityEditor;

namespace DotEditor.Core.Asset
{
    [CustomEditor(typeof(AssetBundleGroupSchema))]
    public class AssetBundleGroupSchemaEditor : BaseGroupSchemaEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawIsEnable();
            DrawGroupInfo();
            EditorGUILayout.Space();
            DrawFilters();
            EditorGUILayout.Space();
            DrawActions();
            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
