using DotEditor.Core.Packer;
using System.Linq;
using UnityEditor;

namespace DotEditor.Core.Asset
{
    public static class AssetBundleSchemaUtil
    {
        public static void UpdatePackConfigBySchema(AssetBundlePackConfig config, AssetBundleSchemaSetting setting)
        {
            config.groupDatas.Clear();

            AssetBundleGroupInput groupInput = new AssetBundleGroupInput() { packConfig = config };
            foreach (var group in setting.groupSchemas)
            {
                group?.Execute(groupInput);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(AssetBundlePackConst.ASSET_BUNDLE_PACK_CONFIG_PATH);
        }

        public static void SetAssetBundleNames(AssetBundlePackConfig config, bool isShowProgressBar = false)
        {
            AssetBundleAssetData[] datas = (from groupData in config.groupDatas
                                       from detailData in groupData.assetDatas
                                       select detailData).ToArray();
            if (isShowProgressBar)
            {
                EditorUtility.DisplayProgressBar("Set Bundle Names", "", 0f);
            }

            if (datas != null && datas.Length > 0)
            {
                for (int i = 0; i < datas.Length; i++)
                {
                    if (isShowProgressBar)
                    {
                        EditorUtility.DisplayProgressBar("Set Bundle Names", datas[i].path, i / (float)datas.Length);
                    }
                    AssetImporter assetImporter = AssetImporter.GetAtPath(datas[i].path);
                    assetImporter.assetBundleName = datas[i].bundle;
                }
            }
            if (isShowProgressBar)
            {
                EditorUtility.ClearProgressBar();
            }

            AssetDatabase.SaveAssets();
        }
    }
}
