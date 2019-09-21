using System.Collections.Generic;
using UnityEngine;

namespace Dot.Core.AssetRuler.AssetAddress
{
    [CreateAssetMenu(fileName = "address_label_operation", menuName = "Asset Ruler/Asset Address/Operation/Set Label")]
    public class AssetAddressLabelOperation : AssetOperation
    {
        public List<string> labels = new List<string>();

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

                addressData.labels = labels.ToArray();
            }

            return result;
        }

    }
}
