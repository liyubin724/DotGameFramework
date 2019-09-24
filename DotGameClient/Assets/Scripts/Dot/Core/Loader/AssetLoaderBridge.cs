using Dot.Core.Pool;
using System;
using System.Collections.Generic;
using SystemObject = System.Object;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public class LoaderBridgeData : IObjectPoolItem
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
        private bool isDisposed = false;
        private static ObjectPool<LoaderBridgeData> brigeDataPool = new ObjectPool<LoaderBridgeData>(10);
        private List<LoaderBridgeData> bridgeDatas = new List<LoaderBridgeData>();
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
                        LoaderBridgeData brigeData = bridgeDatas[i];
                        AssetLoaderHandle handle = brigeData.handle;
                        AssetManager.GetInstance().UnloadAssetLoader(handle, true);
                        bridgeDatas.RemoveAt(i);
                        brigeDataPool.Release(brigeData);
                    }
                }
            }
            isDisposed = true;
        }
        
        public void LoadAssetAsync(string pathOrAddress,OnAssetLoadComplete complete,SystemObject userData = null)
        {
            LoadBatchAssetAsync(new string[] { pathOrAddress }, complete, null,userData);
        }

        public void InstanceAssetAsync(string pathOrAddress, OnAssetLoadComplete complete, SystemObject userData = null)
        {
            InstanceBatchAssetAsync(new string[] { pathOrAddress }, complete, null, userData);
        }

        public void LoadBatchAssetAsync(string[] pathOrAddresses,OnAssetLoadComplete complete,OnBatchAssetLoadComplete batchComplete, SystemObject userData = null)
        {
            AssetLoaderHandle handle = null;
            LoaderBridgeData brigeData = brigeDataPool.Get();
            brigeData.complete = complete;
            brigeData.batchComplete = batchComplete;
            brigeData.userData = userData;

            handle = AssetManager.GetInstance().LoadBatchAssetAsync(pathOrAddresses, AssetLoadComplete, BatchAssetLoadComplete,
                AssetLoaderPriority.Default, null,null, brigeData);

            brigeData.handle = handle;
            bridgeDatas.Add(brigeData);
        }
        
        public void InstanceBatchAssetAsync(string[] pathOrAddresses, OnAssetLoadComplete complete, OnBatchAssetLoadComplete batchComplete, SystemObject userData = null)
        {
            AssetLoaderHandle handle = null;
            LoaderBridgeData brigeData = brigeDataPool.Get();
            brigeData.complete = complete;
            brigeData.batchComplete = batchComplete;
            brigeData.userData = userData;

            handle = AssetManager.GetInstance().InstanceBatchAssetAsync(pathOrAddresses, AssetLoadComplete, BatchAssetLoadComplete,
                AssetLoaderPriority.Default, null, null, brigeData);

            brigeData.handle = handle;
            bridgeDatas.Add(brigeData);
        }

        private void AssetLoadComplete(string pathOrAddress, UnityObject uObj, SystemObject userData)
        {
            LoaderBridgeData brigeData = userData as LoaderBridgeData;
            brigeData.complete?.Invoke(pathOrAddress, uObj, brigeData.userData);
        }

        private void BatchAssetLoadComplete(string[] pathOrAddresses, UnityObject[] uObjs, SystemObject userData)
        {
            LoaderBridgeData brigeData = userData as LoaderBridgeData;
            brigeData.batchComplete?.Invoke(pathOrAddresses, uObjs, brigeData.userData);

            bridgeDatas.Remove(brigeData);
            brigeDataPool.Release(brigeData);
        }
    }
}
