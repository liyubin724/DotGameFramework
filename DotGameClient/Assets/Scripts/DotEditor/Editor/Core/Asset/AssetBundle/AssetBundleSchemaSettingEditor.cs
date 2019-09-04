using DotEditor.Core.Packer;
using UnityEditor;
using UnityEngine;

namespace DotEditor.Core.Asset
{
    [CustomEditor(typeof(AssetBundleSchemaSetting))]
    public class AssetBundleSchemaSettingEditor : BaseAssetSchemaSettingEditor
    {
        private SerializedProperty packConfig;

        protected override void OnEnable()
        {
            base.OnEnable();
            packConfig = serializedObject.FindProperty("packConfig");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PropertyField(packConfig);
                if(GUILayout.Button("find or create",GUILayout.Width(100)))
                {
                    FindOrCreateConfig();
                }
            }
            EditorGUILayout.EndHorizontal();

            DrawSetting();
            DrawGroups();

            EditorGUILayout.Space();
            DrawOperation();
            if(GUILayout.Button("Execute",GUILayout.Height(40)))
            {
                Execute();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void Execute()
        {
            FindOrCreateConfig();

            AssetBundleSchemaSetting setting = target as AssetBundleSchemaSetting;
            AssetBundlePackConfig config = setting.packConfig;
            config.groupDatas.Clear();

            AssetBundleSchemaUtil.UpdatePackConfigBySchema(config, setting);
        }

        private void FindOrCreateConfig()
        {
            if(packConfig.objectReferenceValue == null)
            {
                packConfig.objectReferenceValue = BundlePackUtil.FindOrCreateConfig();
            }
        }

        private void SetAssetBundleNames()
        {
            FindOrCreateConfig();

            AssetBundleSchemaSetting setting = target as AssetBundleSchemaSetting;
            AssetBundlePackConfig config = setting.packConfig;

            AssetBundleSchemaUtil.SetAssetBundleNames(config, true);
        }
    }
}
