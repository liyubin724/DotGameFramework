using System.IO;
using UnityEngine;

namespace Dot.Core.AssetRuler.AssetAddress
{
    [CreateAssetMenu(fileName = "pack_mode_operation", menuName = "Asset Ruler/Asset Address/Operation/Pack Mode")]
    public class AssetAddressPackModeOperation : AssetOperation
    {
        public AssetBundlePackMode packMode = AssetBundlePackMode.Together;
        public int packCount = 0;

        public override AssetOperationResult Execute(AssetFilterResult filterResult, ref AssetOperationResult operationResult)
        {
            if (operationResult == null)
            {
                operationResult = new AssetAddressOperationResult();
            }
            AssetAddressOperationResult result = operationResult as AssetAddressOperationResult;
            foreach (var assetPath in filterResult.assetPaths)
            {
                if (!result.addressDataDic.TryGetValue(assetPath, out AssetBundleAddressData addressData))
                {
                    addressData = new AssetBundleAddressData();
                    addressData.path = assetPath;
                    result.addressDataDic.Add(assetPath, addressData);
                }

                string rootFolder = Path.GetDirectoryName(assetPath).Replace("\\", "/");

                addressData.bundle = GetAssetBundle(rootFolder,assetPath);
            }

            return result;
        }

        private int groupCount = 0;
        private int groupIndex = 0;
        private string GetAssetBundle(string rootFolder, string assetPath)
        {
            if (packMode == AssetBundlePackMode.Separate)
            {
                char[] replaceChars = new char[] { '.', ' ', '\t' };
                foreach (var c in replaceChars)
                {
                    assetPath = assetPath.Replace(c, '_');

                }
                return assetPath;
            }
            else if (packMode == AssetBundlePackMode.Together)
            {
                return rootFolder.Replace(' ', '_');
            }
            else if (packMode == AssetBundlePackMode.GroupByCount)
            {
                groupCount++;
                if (groupCount >= packCount)
                {
                    groupIndex++;
                    groupCount = 0;
                }
                return rootFolder + "_" + groupIndex;
            }
            return null;
        }
    }
}
