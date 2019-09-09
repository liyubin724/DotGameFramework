using Dot.Core.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Core.Loader
{
    public class AssetBundleLoaderData : AssetLoaderData
    {
        private Dictionary<string, List<AssetBundleAsyncOperation>> asyncOperationDic = new Dictionary<string, List<AssetBundleAsyncOperation>>();

        public void AddAsyncOperation(string assetPath,AssetBundleAsyncOperation operation)
        {
            if(!asyncOperationDic.TryGetValue(assetPath,out List<AssetBundleAsyncOperation> operationList))
            {
                operationList = new List<AssetBundleAsyncOperation>();
                asyncOperationDic.Add(assetPath, operationList);
            }
            operationList.Add(operation);
        }

        public bool IsAsyncOperationComplete(string assetPath)
        {
            bool isComplete = true;
            if (asyncOperationDic.TryGetValue(assetPath, out List<AssetBundleAsyncOperation> operationList))
            {
                foreach(var operation in operationList)
                {
                    if(operation.Status != AssetAsyncOperationStatus.Loaded)
                    {
                        isComplete = false;
                        break;
                    }
                }
            }
            return isComplete;
        }

        public bool IsAssetInAsyncOperation(string assetPath)=> asyncOperationDic.ContainsKey(assetPath);

        public AssetBundleAsyncOperation[] GetAllOperation(string assetPath)
        {
            if (asyncOperationDic.TryGetValue(assetPath, out List<AssetBundleAsyncOperation> operationList))
            {
                return operationList.ToArray();
            }
            return null;
        }

        public void DeleteAssetAsyncOperation(string assetPath)
        {
            asyncOperationDic.Remove(assetPath);
        }
        
        public override void OnRelease()
        {
            asyncOperationDic.Clear();
            base.OnRelease();
        }
    }
}
