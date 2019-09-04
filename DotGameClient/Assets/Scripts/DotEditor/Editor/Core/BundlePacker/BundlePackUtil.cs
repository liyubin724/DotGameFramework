using Dot.Core.Loader.Config;
using DotEditor.Core.Asset;
using DotEditor.Core.Util;
using UnityEditor;
using UnityEngine;

namespace DotEditor.Core.Packer
{
    public class BundlePackUtil
    {
        public static AssetBundlePackConfig FindOrCreateConfig()
        {
            AssetBundlePackConfig config = AssetDatabase.LoadAssetAtPath<AssetBundlePackConfig>(AssetBundlePackConst.ASSET_BUNDLE_PACK_CONFIG_PATH);

            bool isNewCreate = false;
            if (config == null)
            {
                isNewCreate = true;
                config = ScriptableObject.CreateInstance<AssetBundlePackConfig>();
                AssetDatabase.CreateAsset(config, AssetBundlePackConst.ASSET_BUNDLE_PACK_CONFIG_PATH);

                AssetDatabase.ImportAsset(AssetBundlePackConst.ASSET_BUNDLE_PACK_CONFIG_PATH);
            }

            if (isNewCreate)
            {
                AssetDatabase.SaveAssets();
            }
            return config;
        }

        public static void UpdatePackConfigBySchema()
        {
            AssetBundlePackConfig config = AssetDatabase.LoadAssetAtPath<AssetBundlePackConfig>(AssetBundlePackConst.ASSET_BUNDLE_PACK_CONFIG_PATH);
            if (config == null)
            {
                Debug.LogError("AssetBundleSchemaUtil::UpdatePackConfigBySchema->config is null;");
                return;
            }

            string[] settingPaths = AssetDatabaseUtil.FindAssets<AssetBundleSchemaSetting>();
            if (settingPaths == null || settingPaths.Length == 0)
            {
                Debug.LogError("AssetBundleSchemaUtil::UpdatePackConfigBySchema->Not found schema Setting;");
                return;
            }

            AssetBundleSchemaSetting setting = AssetDatabase.LoadAssetAtPath<AssetBundleSchemaSetting>(settingPaths[0]);
            if (setting == null)
            {
                Debug.LogError("AssetBundleSchemaUtil::UpdatePackConfigBySchema->Schema Setting is Null.");
                return;
            }

            AssetBundleSchemaUtil.UpdatePackConfigBySchema(config, setting);
        }

        public static void SetAssetBundleNames(bool isShowProgressBar = false)
        {
            AssetBundlePackConfig config = AssetDatabase.LoadAssetAtPath<AssetBundlePackConfig>(AssetBundlePackConst.ASSET_BUNDLE_PACK_CONFIG_PATH);
            if (config == null)
            {
                Debug.LogError("AssetBundleSchemaUtil::SetAssetBundleNames->config is null;");
                return;
            }

            AssetImporter assetImporter = AssetImporter.GetAtPath(AssetBundlePackConst.ASSET_BUNDLE_PACK_CONFIG_PATH);
            assetImporter.assetBundleName = typeof(AssetBundlePackConfig).Name.ToLower();

            AssetBundleSchemaUtil.SetAssetBundleNames(config, isShowProgressBar);

        }

        public static void ClearAssetBundleNames(bool isShowProgressBar = false)
        {
            string[] assetPaths = AssetDatabaseUtil.FindAssetWithBundleName();
            if (assetPaths != null && assetPaths.Length > 0)
            {
                EditorUtility.DisplayProgressBar("Clear Asset Bundle Names", "", 0.0f);
                for(int i =0;i<assetPaths.Length;i++)
                {
                    EditorUtility.DisplayProgressBar("Clear Asset Bundle Names", assetPaths[i], i/(float)assetPaths.Length);
                    AssetImporter assetImporter = AssetImporter.GetAtPath(assetPaths[i]);
                    assetImporter.assetBundleName = "";
                }
                EditorUtility.ClearProgressBar();
            }

            AssetDatabase.RemoveUnusedAssetBundleNames();

            AssetDatabase.SaveAssets();
        }


    }
}

