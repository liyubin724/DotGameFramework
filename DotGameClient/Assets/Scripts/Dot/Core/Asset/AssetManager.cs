using Dot.Core.Generic;
using Dot.Core.Util;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Asset
{
    public delegate void OnAssetLoadFinishCallback(string assetPath, UnityObject uObj);
    public delegate void OnAssetLoadProgressCallback(string assetPath, float progress);
    public delegate void OnAssetsLoadFinishCallback(string[] assetPaths, UnityObject[] uObj);
    public delegate void OnAssetsLoadProgressCallback(string[] assetPaths, float[] progresses);

    public class AssetManager : Singleton<AssetManager>
    {
        private UniqueIDCreator idCreator = new UniqueIDCreator();
        private List<LoadData> loadDataList = new List<LoadData>();
        private Dictionary<string, AssetData> assetDataDic = new Dictionary<string, AssetData>();
        private List<AssetData> loadingAssetDataList = new List<AssetData>();
        private Dictionary<string, InstanceAssetData> instanceAssetDataDic = new Dictionary<string, InstanceAssetData>();

        
        public int MaxLoadingCount { get; set; } = 5;

        public void DoUpdate()
        {
            UpdateLoadingData();
            UpdateLoadData();
            ClearInstanceAssetData();
            ClearAssetData();
        }

        private List<string> removedInstanceAssetKeys = new List<string>();
        private void ClearInstanceAssetData()
        {
            foreach(var kvp in instanceAssetDataDic)
            {
                if(!kvp.Value.IsInUsed())
                {
                    removedInstanceAssetKeys.Add(kvp.Key);
                }
            }

            removedInstanceAssetKeys.ForEach((key) =>
            {
                instanceAssetDataDic[key].Release();
                instanceAssetDataDic.Remove(key);
            });
            removedInstanceAssetKeys.Clear();
        }

        private void UpdateLoadingData()
        {
            int updateIndex = 0;
            while(updateIndex < loadingAssetDataList.Count && updateIndex<MaxLoadingCount)
            {
                AssetData assetData = loadingAssetDataList[updateIndex];
                if(assetData.Status == AssetDataStatus.None)
                {
                    assetData.Handle = Addressables.LoadAssetAsync<UnityObject>(assetData.Address);
                    assetData.Handle.Completed += OnAssetLoadComplete;
                    assetData.Status = AssetDataStatus.Loading;

                    ++updateIndex;
                }
                else if(assetData.Status == AssetDataStatus.Loaded)
                {
                    loadingAssetDataList.RemoveAt(updateIndex);
                }else
                {
                    ++updateIndex;
                }
            }
        }

        
        private UnityObject GetObjectInstance(string address,AssetData assetData)
        {
            if(instanceAssetDataDic.TryGetValue(address,out InstanceAssetData instance))
            {
                return instance.GetInstance();
            }else
            {
                InstanceAssetData iaData = new InstanceAssetData(assetData);
                instanceAssetDataDic.Add(address, iaData);

                return iaData.GetInstance();
            }
        }

        private void UpdateLoadData()
        {
            for(int i = loadDataList.Count-1;i>=0;--i)
            {
                LoadData loadData = loadDataList[i];

                bool isAllFinished = true;
                float[] progresses = new float[loadData.addresses.Length];
                for(int j =0;j<loadData.addresses.Length;++j)
                {
                    string address = loadData.addresses[j];
                    AssetData assetData = assetDataDic[address];
                    if(assetData.Status == AssetDataStatus.Loaded && !loadData.GetIsSingleFinishCalled(j))
                    {
                        assetData.ReleaseLoadCount();
                        progresses[j] = 1.0f;
                        if(loadData.isInstance)
                        {
                            loadData.SetObject(j, GetObjectInstance(address, assetData));
                        }else
                        {
                            loadData.SetObject(j, assetData.GetObject());
                            assetData.RetainRefCount();
                        }

                        loadData.SetIsSingleFinishCalled(j, true);
                        loadData.InvokeAssetLoadProgress(address, 1.0f);
                        loadData.InvokeAssetLoadFinish(loadData.addresses[j], loadData.GetObject(j));
                    }
                    else if(assetData.Status == AssetDataStatus.Loading)
                    {
                        progresses[j] = assetData.Handle.PercentComplete;
                        if (isAllFinished) isAllFinished = false;

                        loadData.InvokeAssetLoadProgress(address, progresses[j]);
                    }
                    else
                    {
                        progresses[j] = 0.0f;
                        if (isAllFinished) isAllFinished = false;
                        loadData.InvokeAssetLoadProgress(address, 0.0f);
                    }
                }

                loadData.InvokeAssetsLoadProgress(progresses);
                if(isAllFinished)
                {
                    loadData.InvokeAssetsLoadFinish(loadData.Objects);

                    loadDataList.RemoveAt(i);
                }
            }
        }

        private List<string> removedAssetAdress = new List<string>();
        private void ClearAssetData()
        {
            foreach(var kvp in assetDataDic)
            {
                if(!kvp.Value.IsValid())
                {
                    removedAssetAdress.Add(kvp.Key);
                }
            }
            removedAssetAdress.ForEach((address) =>
            {
                assetDataDic[address].Unload();
                assetDataDic.Remove(address);
            });
            removedAssetAdress.Clear();
        }

        public AssetHandle LoadAssetAsync(string address,
            OnAssetLoadFinishCallback finish,
            OnAssetLoadProgressCallback progress)
        {
            return LoadAssetsAsync(new string[] { address }, finish, progress, null, null);
        }

        public AssetHandle LoadAssetsAsync(string[] addresses,
            OnAssetLoadFinishCallback singleFinish,
            OnAssetLoadProgressCallback singleProgress,
            OnAssetsLoadFinishCallback allFinish,
            OnAssetsLoadProgressCallback allProgress)
        {
            return LoadAsync(addresses, false, singleFinish, singleProgress, allFinish, allProgress);
        }
        
        public void InstanceAssetAsync(string address,
            OnAssetLoadFinishCallback finish,
            OnAssetLoadProgressCallback progress) 
        {
            InstanceAssetsAsync(new string[] { address }, finish, progress, null, null);
        }

        public void InstanceAssetsAsync(string[] addresses,
             OnAssetLoadFinishCallback singleFinish,
            OnAssetLoadProgressCallback singleProgress,
            OnAssetsLoadFinishCallback allFinish,
            OnAssetsLoadProgressCallback allProgress)
        {
            LoadAsync(addresses, true, singleFinish, singleProgress, allFinish, allProgress);
        }

        private AssetHandle LoadAsync(string[] addresses,bool isInstance,
            OnAssetLoadFinishCallback singleFinish,
            OnAssetLoadProgressCallback singleProgress,
            OnAssetsLoadFinishCallback allFinish,
            OnAssetsLoadProgressCallback allProgress)
        {
            LoadData loadData = new LoadData();
            loadData.uniqueID = idCreator.Next();
            loadData.SetData(addresses, isInstance, singleFinish, allFinish, singleProgress, allProgress);

            foreach (var address in addresses)
            {
                if (assetDataDic.TryGetValue(address, out AssetData assetData))
                {
                    assetData.RetainLoadCount();
                }
                else
                {
                    assetData = new AssetData();
                    assetData.Address = address;
                    assetData.RetainLoadCount();

                    assetDataDic.Add(address, assetData);
                    loadingAssetDataList.Add(assetData);
                }
            }
            loadDataList.Add(loadData);
            if(!isInstance)
            {
                AssetHandle assetHandle = new AssetHandle() { uniqueID = loadData.uniqueID, addresses = addresses, };
                assetHandle.releaseAction = ReleaseAsset;
                return assetHandle;
            }
            return null;
        }

        public void ReleaseAsset(AssetHandle assetHandle)
        {
            if(!assetHandle.IsValid)
            {
                return;
            }

            LoadData loadData = null;
            foreach(var data in loadDataList)
            {
                if(data.uniqueID == assetHandle.uniqueID)
                {
                    loadData = data;
                    break;
                }
            }

            if(loadData!=null)
            {
                ReleaseAssetByLoadData(loadData);
            }else
            {
                foreach(var address in assetHandle.addresses)
                {
                    AssetData assetData = assetDataDic[address];
                    assetData.ReleaseRefCount();
                }
                assetHandle.IsValid = false;
            }
        }

        private void ReleaseAssetByLoadData(LoadData loadData)
        {
            for (int i = 0; i < loadData.addresses.Length; i++)
            {
                AssetData assetData = assetDataDic[loadData.addresses[i]];

                UnityObject uObj = loadData.GetObject(i);
                if (uObj != null)
                {
                    if (loadData.isInstance)
                    {
                        UnityObject.Destroy(uObj);
                    }
                    else
                    {
                        assetData.ReleaseRefCount();
                    }
                }
                else
                {
                    assetData.ReleaseLoadCount();
                }
            }
        }


        private void OnAssetLoadComplete(AsyncOperationHandle handle)
        {
            foreach(var assetData in loadingAssetDataList)
            {
                if(handle.Equals( assetData.Handle))
                {
                    assetData.Handle.Completed -= OnAssetLoadComplete;
                    assetData.Status = AssetDataStatus.Loaded;
                }
            }
        }


    }
    
    

}
