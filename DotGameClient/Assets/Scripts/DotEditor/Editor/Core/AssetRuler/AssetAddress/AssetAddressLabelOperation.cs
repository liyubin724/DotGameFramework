using Dot.Core.Loader.Config;
using System.Collections.Generic;
using UnityEngine;

namespace DotEditor.Core.AssetRuler.AssetAddress
{
    [CreateAssetMenu(fileName = "address_label_operation", menuName = "Asset Address/Operations/Set Label")]
    public class AssetAddressLabelOperation : AssetOperation
    {
        public List<string> labels = new List<string>();

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

                addressData.labels = labels.ToArray();
            }

            return result;
        }

    }
}
