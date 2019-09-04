using DotEditor.Core.Packer;
using UnityEditor;
using UnityEngine;

namespace DotEditor.Core.Asset
{
    [CustomEditor(typeof(AssetBundleSchemaSetting))]
    public class AssetBundleSchemaSettingEditor : BaseAssetSchemaSettingEditor
    {
        private SerializedProperty tagConfig;

        protected override void OnEnable()
        {
            base.OnEnable();
            tagConfig = serializedObject.FindProperty("tagConfig");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PropertyField(tagConfig);
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
            AssetBundleTagConfig config = setting.tagConfig;
            config.groupDatas.Clear();

            AssetBundleSchemaUtil.UpdateTagConfigBySchema(config, setting);
        }

        private void FindOrCreateConfig()
        {
            if(tagConfig.objectReferenceValue == null)
            {
                tagConfig.objectReferenceValue = BundlePackUtil.FindOrCreateTagConfig();
            }
        }

        private void SetAssetBundleNames()
        {
            FindOrCreateConfig();

            AssetBundleSchemaSetting setting = target as AssetBundleSchemaSetting;
            AssetBundleTagConfig config = setting.tagConfig;

            AssetBundleSchemaUtil.SetAssetBundleNames(config, true);
        }
    }
}
