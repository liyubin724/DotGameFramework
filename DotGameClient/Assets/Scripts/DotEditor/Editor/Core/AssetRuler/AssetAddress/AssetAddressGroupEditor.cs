using UnityEditor;

namespace DotEditor.Core.AssetRuler.AssetAddress
{
    [CustomEditor(typeof(AssetAddressGroup))]
    public class AssetAddressGroupEditor :AssetGroupEditor
    {
        private SerializedProperty isMain;
        private SerializedProperty isPreload;

        protected override void OnEnable()
        {
            base.OnEnable();
            isMain = serializedObject.FindProperty("isMain");
            isPreload = serializedObject.FindProperty("isPreload");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawBaseInfo();
            EditorGUILayout.PropertyField(isMain);
            EditorGUILayout.PropertyField(isPreload);
            DrawAssetSearcher();
            DrawFilterOperations();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
