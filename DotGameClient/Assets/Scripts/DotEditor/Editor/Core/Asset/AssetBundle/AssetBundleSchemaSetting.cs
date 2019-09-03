using Dot.Core.Loader.Config;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DotEditor.Core.Asset
{
    [CreateAssetMenu(fileName = "assetbundle_asset_setting", menuName = "Asset/Asset Setting/Asset Bundle")]
    public class AssetBundleSchemaSetting :  BaseAssetSchemaSetting
    {
        [ReadOnly]
        [HorizontalGroup("assetDetailConfig")]
        public AssetDetailConfig assetDetailConfig;

        [HorizontalGroup("assetDetailConfig", 80)]
        [Button("find or create")]
        public void CreateAssetDetailConfig()
        {
            AssetDetailConfig config = AssetDatabase.LoadAssetAtPath<AssetDetailConfig>(AssetDetailConst.ASSET_DETAIL_CONFIG_PATH);
            bool isNewCreate = false;
            if(config == null)
            {
                isNewCreate = true;
                config = ScriptableObject.CreateInstance<AssetDetailConfig>();
                AssetDatabase.CreateAsset(config, AssetDetailConst.ASSET_DETAIL_CONFIG_PATH);
            }
            assetDetailConfig = config;
            if(isNewCreate)
            {
                AssetDatabase.SaveAssets();
            }
        }

        [PropertyOrder(300)]
        [Button("Execute", ButtonSizes.Large)]
        public void Execute()
        {
            if(assetDetailConfig == null)
            {
                CreateAssetDetailConfig();
            }
            assetDetailConfig.assetGroupDatas.Clear();
            AssetBundleGroupInput groupInput = new AssetBundleGroupInput() { detailConfig = assetDetailConfig };
            foreach (var group in groupSchemas)
            {
                group.Execute(groupInput);
            }
            AssetDatabase.SaveAssets();
        }

        [PropertyOrder(300)]
        [Button("SetAssetBundleName", ButtonSizes.Large)]
        public void SetAssetBundleName()
        {
            Execute();

        }
    }
}
