using Dot.Core.Loader.Config;
using System.IO;
using UnityEngine;

namespace DotEditor.Core.AssetRuler.AssetAddress
{
    [CreateAssetMenu(fileName = "address_mode_operation", menuName = "Asset Ruler/Asset Address/Operation/Address Mode")]
    public class AssetAddressAddressModeOperation : AssetOperation
    {
        public AssetAddressMode addressMode = AssetAddressMode.FileNameWithoutExtension;
        public string fileNameFormat = "{0}";

        public override AssetOperationResult Execute(AssetFilterResult filterResult, AssetOperationResult operationResult)
        {
            if(operationResult == null)
            {
                operationResult = new AssetAddressOperationResult();
            }
            AssetAddressOperationResult result = operationResult as AssetAddressOperationResult;
            foreach (var assetPath in filterResult.assetPaths)
            {
                if(!result.addressDataDic.TryGetValue(assetPath,out AssetAddressData addressData))
                {
                    addressData = new AssetAddressData();
                    addressData.assetPath = assetPath;
                    result.addressDataDic.Add(assetPath, addressData);
                }

                addressData.assetAddress = GetAssetAddress(assetPath);
            }

            return result;
        }

        private string GetAssetAddress(string assetPath)
        {
            if (addressMode == AssetAddressMode.FullPath)
                return assetPath;
            else if (addressMode == AssetAddressMode.FileName)
                return Path.GetFileName(assetPath);
            else if (addressMode == AssetAddressMode.FileNameWithoutExtension)
                return Path.GetFileNameWithoutExtension(assetPath);
            else if (addressMode == AssetAddressMode.FileFormatName)
                return string.Format(fileNameFormat, Path.GetFileNameWithoutExtension(assetPath));
            else
                return assetPath;
        }
    }
}
