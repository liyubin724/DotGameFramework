using Dot.Core.Loader.Config;
using UnityEditor;
using UnityEngine;
using System.Linq;
using DotEditor.Core.Packer;

namespace DotEditor.Core.Asset
{
    [CustomEditor(typeof(AssetBundleSchemaSetting))]
    public class AssetBundleSchemaSettingEditor : BaseAssetSchemaSettingEditor
    {
        private SerializedProperty assetDetailConfig;

        protected override void OnEnable()
        {
            base.OnEnable();
            assetDetailConfig = serializedObject.FindProperty("assetDetailConfig");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PropertyField(assetDetailConfig);
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

            if (GUILayout.Button("Set Asset Bundle Names", GUILayout.Height(40)))
            {
                SetAssetBundleNames();
            }

            if (GUILayout.Button("Clear Asset Bundle Names", GUILayout.Height(40)))
            {
                BundlePackUtil.ClearAssetBundleNames();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void Execute()
        {
            FindOrCreateConfig();

            AssetBundleSchemaSetting setting = target as AssetBundleSchemaSetting;
            AssetDetailConfig config = setting.assetDetailConfig;
            config.assetGroupDatas.Clear();

            AssetBundleSchemaUtil.UpdateAssetDetailConfigBySchema(config, setting);
        }

        private void FindOrCreateConfig()
        {
            if(assetDetailConfig.objectReferenceValue == null)
            {
                AssetDetailConfig config = AssetDatabase.LoadAssetAtPath<AssetDetailConfig>(AssetDetailConst.ASSET_DETAIL_CONFIG_PATH);

                bool isNewCreate = false;
                if (config == null)
                {
                    isNewCreate = true;
                    config = ScriptableObject.CreateInstance<AssetDetailConfig>();
                    AssetDatabase.CreateAsset(config, AssetDetailConst.ASSET_DETAIL_CONFIG_PATH);
                }

                assetDetailConfig.objectReferenceValue = config;

                if (isNewCreate)
                {
                    AssetDatabase.SaveAssets();
                }
            }
        }

        private void SetAssetBundleNames()
        {
            FindOrCreateConfig();

            AssetBundleSchemaSetting setting = target as AssetBundleSchemaSetting;
            AssetDetailConfig config = setting.assetDetailConfig;

            AssetBundleSchemaUtil.SetAssetBundleNames(config, true);
        }
    }
}
