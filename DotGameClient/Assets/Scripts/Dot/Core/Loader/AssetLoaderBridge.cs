using Dot.Core.Pool;
using System;
using System.Collections.Generic;
using SystemObject = System.Object;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    internal class BridgeData : IObjectPoolItem
    {
        public AssetLoaderHandle handle;
        public OnAssetLoadComplete complete;
        public OnBatchAssetLoadComplete batchComplete;
        public SystemObject userData;

        public void OnNew()
        {
        }

        public void OnRelease()
        {
            handle = null;
            complete = null;
            batchComplete = null;
            userData = null;
        }
    }

    public class AssetLoaderBridge : IDisposable
    {
        private static ObjectPool<BridgeData> brigeDataPool = new ObjectPool<BridgeData>(10);

        private bool isDisposed = false;
        private List<BridgeData> bridgeDatas = new List<BridgeData>();
        private AssetLoaderPriority loaderPriority = AssetLoaderPriority.Default;

        public AssetLoaderBridge()
        {
        }

        public AssetLoaderBridge(AssetLoaderPriority priority)
        {
            loaderPriority = priority;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AssetLoaderBridge()
        {
            Dispose(false);
        }

        private void Dispose(bool isDisposing)
        {
            if (isDisposed) return;
            if(isDisposing)
            {
                if(bridgeDatas.Count>0)
                {
                    for(int i = bridgeDatas.Count-1;i>=0;--i)
                    {
                        CancelLoader(bridgeDatas[i]);
                    }
                }
            }
            isDisposed = true;
        }

        private void CancelLoader(BridgeData bridgeData)
        {
            AssetLoaderHandle handle = bridgeData.handle;
            if (bridgeData.complete != null)
            {
                AssetManager.GetInstance().UnloadAssetLoader(handle, false);
            }
            else
            {
                AssetManager.GetInstance().UnloadAssetLoader(handle, true);
            }
            bridgeDatas.Remove(bridgeData);
            brigeDataPool.Release(bridgeData);
        }
        
        public void LoadAssetAsync(string pathOrAddress,OnAssetLoadComplete complete,SystemObject userData = null)
        {
            LoadBatchAssetAsync(new string[] { pathOrAddress }, complete, null,userData);
        }

        public void InstanceAssetAsync(string pathOrAddress, OnAssetLoadComplete complete, SystemObject userData = null)
        {
            InstanceBatchAssetAsync(new string[] { pathOrAddress }, complete, null, userData);
        }

        public void CancelLoadAsset(OnAssetLoadComplete complete)
        {
            CancelLoadAsset(complete, null);
        }

        public void CancelLoadAsset(OnBatchAssetLoadComplete batchComplete)
        {
            CancelLoadAsset(null, batchComplete);
        }

        public void CancelLoadAsset(OnAssetLoadComplete complete, OnBatchAssetLoadComplete batchComplete)
        {
            if(complete == null && batchComplete == null)
            {
                return;
            }

            for(int i = bridgeDatas.Count;i>=0;--i)
            {
                BridgeData bridgeData = bridgeDatas[i];
                bool isSame = true;
                if(complete!=null)
                {
                    isSame = bridgeData.complete == complete;
                }
                if(isSame && batchComplete!=null)
                {
                    isSame = bridgeData.batchComplete == batchComplete;
                }
                if(isSame)
                {
                    CancelLoader(bridgeData);
                }
            }
        }

        public void LoadBatchAssetAsync(string[] pathOrAddresses,OnAssetLoadComplete complete,OnBatchAssetLoadComplete batchComplete, SystemObject userData = null)
        {
            BridgeData brigeData = brigeDataPool.Get();
            brigeData.complete = complete;
            brigeData.batchComplete = batchComplete;
            brigeData.userData = userData;

            AssetLoaderHandle handle = AssetManager.GetInstance().LoadBatchAssetAsync(pathOrAddresses, AssetLoadComplete, BatchAssetLoadComplete,
                loaderPriority, null,null, brigeData);

            brigeData.handle = handle;

            bridgeDatas.Add(brigeData);
        }
        
        public void InstanceBatchAssetAsync(string[] pathOrAddresses, OnAssetLoadComplete complete, OnBatchAssetLoadComplete batchComplete, SystemObject userData = null)
        {
            BridgeData brigeData = brigeDataPool.Get();
            brigeData.complete = complete;
            brigeData.batchComplete = batchComplete;
            brigeData.userData = userData;

            AssetLoaderHandle handle = AssetManager.GetInstance().InstanceBatchAssetAsync(pathOrAddresses, AssetLoadComplete, BatchAssetLoadComplete,
                loaderPriority, null, null, brigeData);

            brigeData.handle = handle;

            bridgeDatas.Add(brigeData);
        }

        private void AssetLoadComplete(string pathOrAddress, UnityObject uObj, SystemObject userData)
        {
            BridgeData brigeData = userData as BridgeData;
            brigeData.complete?.Invoke(pathOrAddress, uObj, brigeData.userData);
        }

        private void BatchAssetLoadComplete(string[] pathOrAddresses, UnityObject[] uObjs, SystemObject userData)
        {
            BridgeData bridgeData = userData as BridgeData;
            bridgeData.batchComplete?.Invoke(pathOrAddresses, uObjs, bridgeData.userData);

            bridgeDatas.Remove(bridgeData);
            brigeDataPool.Release(bridgeData);
        }
    }
}
