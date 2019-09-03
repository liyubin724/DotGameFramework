using Dot.Core.Loader.Config;
using System.Linq;
using UnityEditor;

namespace DotEditor.Core.Asset
{
    public static class AssetBundleSchemaUtil
    {
        public static void UpdateAssetDetailConfigBySchema(AssetDetailConfig config, AssetBundleSchemaSetting setting)
        {
            config.assetGroupDatas.Clear();

            AssetBundleGroupInput groupInput = new AssetBundleGroupInput() { detailConfig = config };
            foreach (var group in setting.groupSchemas)
            {
                group?.Execute(groupInput);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(AssetDetailConst.ASSET_DETAIL_CONFIG_PATH);
        }

        public static void SetAssetBundleNames(AssetDetailConfig config, bool isShowProgressBar = false)
        {
            AssetDetailData[] datas = (from groupData in config.assetGroupDatas
                                       from detailData in groupData.assetDetailDatas
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
