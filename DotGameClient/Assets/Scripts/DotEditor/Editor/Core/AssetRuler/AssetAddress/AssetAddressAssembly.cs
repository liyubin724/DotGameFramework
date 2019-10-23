using Dot.Core.Loader.Config;
using DotEditor.Core.Packer;
using System.Collections.Generic;
using UnityEngine;

namespace DotEditor.Core.AssetRuler.AssetAddress
{
    [CreateAssetMenu(fileName = "assetaddress_assembly", menuName = "Asset Ruler/Asset Address/Assembly", order = 1)]
    public class AssetAddressAssembly : AssetAssembly
    {
        public override AssetAssemblyResult Execute()
        {
            AssetAddressAssemblyResult result = new AssetAddressAssemblyResult();
            foreach(var group in assetGroups)
            {
                group.Execute(result);
            }

            AssetBundleTagConfig tagConfig = Util.FileUtil.ReadFromBinary<AssetBundleTagConfig>(BundlePackUtil.GetTagConfigPath());
            tagConfig.groupDatas.Clear();

            foreach (var groupResult in result.groupResults)
            {
                AssetAddressGroupResult gResult = groupResult as AssetAddressGroupResult;

                AssetBundleGroupData groupData = new AssetBundleGroupData();
                groupData.groupName = gResult.groupName;
                groupData.isGenAddress = gResult.isGenAddress;
                groupData.isMain = gResult.isMain;
                groupData.isPreload = gResult.isPreload;

                tagConfig.groupDatas.Add(groupData);

                foreach (var operationResult in gResult.operationResults)
                {
                    AssetAddressOperationResult oResult = operationResult as AssetAddressOperationResult;
                    foreach (var kvp in oResult.addressDataDic)
                    {
                        AssetAddressData aaData = new AssetAddressData();
                        AssetAddressData kvpValue = kvp.Value as AssetAddressData;

                        aaData.assetAddress = kvpValue.assetAddress;
                        aaData.assetPath = kvpValue.assetPath;
                        aaData.bundlePath = kvpValue.bundlePath;
                        aaData.labels = new List<string>(kvpValue.labels).ToArray();

                        groupData.assetDatas.Add(aaData);
                    }
                }
            }

            Util.FileUtil.SaveToBinary<AssetBundleTagConfig>(BundlePackUtil.GetTagConfigPath(), tagConfig);

            return result;
        }
    }
}
