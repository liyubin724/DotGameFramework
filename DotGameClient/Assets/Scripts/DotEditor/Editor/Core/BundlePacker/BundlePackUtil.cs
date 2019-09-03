using Dot.Core.Loader.Config;
using DotEditor.Core.Asset;
using DotEditor.Core.Util;
using UnityEditor;
using UnityEngine;

namespace DotEditor.Core.Packer
{
    public class BundlePackUtil
    {
        public static AssetDetailConfig FindOrCreateConfig()
        {
            AssetDetailConfig config = AssetDatabase.LoadAssetAtPath<AssetDetailConfig>(AssetDetailConst.ASSET_DETAIL_CONFIG_PATH);

            bool isNewCreate = false;
            if (config == null)
            {
                isNewCreate = true;
                config = ScriptableObject.CreateInstance<AssetDetailConfig>();
                AssetDatabase.CreateAsset(config, AssetDetailConst.ASSET_DETAIL_CONFIG_PATH);

                AssetDatabase.ImportAsset(AssetDetailConst.ASSET_DETAIL_CONFIG_PATH);
            }

            if (isNewCreate)
            {
                AssetDatabase.SaveAssets();
            }
            return config;
        }

        public static void UpdateAssetDetailConfigBySchema()
        {
            AssetDetailConfig config = AssetDatabase.LoadAssetAtPath<AssetDetailConfig>(AssetDetailConst.ASSET_DETAIL_CONFIG_PATH);
            if (config == null)
            {
                Debug.LogError("AssetBundleSchemaUtil::UpdateAssetDetailConfigBySchema->config is null;");
                return;
            }

            string[] settingPaths = AssetDatabaseUtil.FindAssets<AssetBundleSchemaSetting>();
            if (settingPaths == null || settingPaths.Length == 0)
            {
                Debug.LogError("AssetBundleSchemaUtil::UpdateAssetDetailConfigBySchema->Not found schema Setting;");
                return;
            }

            AssetBundleSchemaSetting setting = AssetDatabase.LoadAssetAtPath<AssetBundleSchemaSetting>(settingPaths[0]);
            if (setting == null)
            {
                Debug.LogError("AssetBundleSchemaUtil::UpdateAssetDetailConfigBySchema->Schema Setting is Null.");
                return;
            }

            AssetBundleSchemaUtil.UpdateAssetDetailConfigBySchema(config, setting);
        }

        public static void SetAssetBundleNames()
        {
            AssetDetailConfig config = AssetDatabase.LoadAssetAtPath<AssetDetailConfig>(AssetDetailConst.ASSET_DETAIL_CONFIG_PATH);
            if (config == null)
            {
                Debug.LogError("AssetBundleSchemaUtil::SetAssetBundleNames->config is null;");
                return;
            }

            AssetImporter assetImporter = AssetImporter.GetAtPath(AssetDetailConst.ASSET_DETAIL_CONFIG_PATH);
            assetImporter.assetBundleName = typeof(AssetDetailConfig).Name.ToLower();

            AssetBundleSchemaUtil.SetAssetBundleNames(config);

        }

        public static void ClearAssetBundleNames()
        {
            string[] assetPaths = AssetDatabaseUtil.FindAssetWithBundleName();
            if (assetPaths != null && assetPaths.Length > 0)
            {
                foreach (var assetPath in assetPaths)
                {
                    AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);
                    assetImporter.assetBundleName = "";
                }
            }

            AssetDatabase.RemoveUnusedAssetBundleNames();

            AssetDatabase.SaveAssets();
        }


    }
}

