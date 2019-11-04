using Dot.Core.Loader.Config;
using Dot.Core.Util;
using System.IO;
using UnityEngine;

namespace DotEditor.Core.AssetRuler.AssetAddress
{
    [CreateAssetMenu(fileName = "pack_mode_operation", menuName = "Asset Address/Operations/Bundle Pack Mode")]
    public class AssetAddressPackModeOperation : AssetOperation
    {
        public AssetBundlePackMode packMode = AssetBundlePackMode.Together;
        public int packCount = 0;
        public string packName = "";

        public override AssetOperationResult Execute(AssetFilterResult filterResult, AssetOperationResult operationResult)
        {
            if (operationResult == null)
            {
                operationResult = new AssetAddressOperationResult();
            }
            AssetAddressOperationResult result = operationResult as AssetAddressOperationResult;
            foreach (var assetPath in filterResult.assetPaths)
            {
                if (!result.addressDataDic.TryGetValue(assetPath, out AssetAddressData addressData))
                {
                    addressData = new AssetAddressData();
                    addressData.assetPath = assetPath;
                    result.addressDataDic.Add(assetPath, addressData);
                }

                string rootFolder = Path.GetDirectoryName(assetPath).Replace("\\", "/");
                addressData.bundlePath = GetAssetBundle(rootFolder,assetPath).ToLower();
            }

            return result;
        }

        private int groupCount = 0;
        private int groupIndex = 0;
        private string GetAssetBundle(string rootFolder, string assetPath)
        {
            string path = StringUtil.RemoveSpecialCharacter(assetPath, "_");
            string folder = StringUtil.RemoveSpecialCharacter(rootFolder, "_");

            if (packMode == AssetBundlePackMode.Separate)
            {
                return path;
            }
            else if (packMode == AssetBundlePackMode.Together)
            {
                return folder;
            }
            else if (packMode == AssetBundlePackMode.GroupByCount)
            {
                groupCount++;
                if (groupCount >= packCount)
                {
                    groupIndex++;
                    groupCount = 0;
                }
                return folder + "_" + groupIndex;
            }else if(packMode == AssetBundlePackMode.TogetherAppendName)
            {
                return folder + "/"+ packName;
            }else if(packMode == AssetBundlePackMode.TogetherWithNewName)
            {
                return packName;
            }
            return null;
        }
    }
}
