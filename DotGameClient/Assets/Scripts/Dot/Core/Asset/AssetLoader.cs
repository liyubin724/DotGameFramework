using Dot.Core.Generic;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using SystemObject = System.Object;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Asset
{
    public delegate void OnAssetLoadFinishCallback(string assetPath, UnityObject uObj,SystemObject userData);
    public delegate void OnAssetLoadProgressCallback(string assetPath, float progress,SystemObject userData);
    public delegate void OnAssetsLoadFinishCallback(string[] assetPaths, UnityObject[] uObj, SystemObject userData);
    public delegate void OnAssetsLoadProgressCallback(string[] assetPaths, float[] progresses,SystemObject userData);

    public class AssetLoader:Util.Singleton<AssetLoader>
    {
        private UniqueIDCreator idCreator = new UniqueIDCreator();
        private AsyncOperationHandle<IResourceLocator> initHandle;

        private IndexMapORM<long, LoadData> loadDataORM = new IndexMapORM<long, LoadData>();
        private Dictionary<string, AssetData> assetDataDic = new Dictionary<string, AssetData>();
        private List<AssetData> loadingAssetDataList = new List<AssetData>();
        private Dictionary<string, InstanceAssetData> instanceAssetDataDic = new Dictionary<string, InstanceAssetData>();

        public int MaxLoadingCount { get; set; } = 5;

        public void Initialize(Action<bool> initCallback)
        {
            initHandle = Addressables.InitializeAsync();
            initHandle.Completed += (handle) =>
            {
                if(handle.Status == AsyncOperationStatus.Succeeded)
                {
                    initCallback?.Invoke(true);
                }else
                {
                    initCallback?.Invoke(false);
                }
            };
        }

        public void DoUpdate()
        {
            UpdateLoadingAsset();
            UpdateLoadData();
            ClearInstanceAssetData();
            ClearAssetData();
            CheckUnloadOperation();
        }

        private void UpdateLoadingAsset()
        {
            int updateIndex = 0;
            while (updateIndex < loadingAssetDataList.Count && updateIndex < MaxLoadingCount)
            {
                AssetData assetData = loadingAssetDataList[updateIndex];
                if (assetData.Status == AssetDataStatus.None)
                {
                    assetData.Handle = Addressables.LoadAssetAsync<UnityObject>(assetData.Address);
                    assetData.Status = AssetDataStatus.Loading;
                    assetData.Handle.Completed += OnAssetLoadComplete;

                    ++updateIndex;
                }
                else if (assetData.Status == AssetDataStatus.Loaded)
                {
                    loadingAssetDataList.RemoveAt(updateIndex);
                }
                else
                { 
                    ++updateIndex;
                }
            }
        }

        private void UpdateLoadData()
        {
            int index = 0;
            while (index < loadDataORM.Count)
            {
                LoadData loadData = loadDataORM.GetDataByIndex(index);
                if (loadData.Addresses == null)
                {
                    ++index;
                    continue;
                }

                bool isFinished = false;
                for (int j = 0; j < loadData.Addresses.Length; ++j)
                {
                    string address = loadData.Addresses[j];
                    AssetData assetData = assetDataDic[address];

                    if (assetData.Status == AssetDataStatus.Loaded)
                    {
                        if(!loadData.GetIsObjectLoaded(j))
                        {
                            loadData.SetObjectLoaded(j);

                            isFinished = loadData.IsFinish();
                            if(isFinished)
                            {
                                loadDataORM.DeleteByData(loadData);
                            }

                            loadData.SetProgress(j, 1.0f);
                            assetData.ReleaseLoadCount();
                            if (loadData.isInstance)
                            {
                                loadData.SetObject(j, GetObjectInstance(address, assetData));
                            }
                            else
                            {
                                assetData.RetainRefCount();
                                loadData.SetObject(j, assetData.GetObject());
                            }
                        }
                    }
                    else if (assetData.Status == AssetDataStatus.Loading)
                    {
                        loadData.SetProgress(j, assetData.Handle.PercentComplete);
                    }
                    else
                    {
                        loadData.SetProgress(j, 0.0f);
                    }
                }

                if(isFinished)
                {
                    loadData.LoadFinish();
                }else
                {
                    ++index;
                }
            }
        }

        private List<string> removedAssetAdress = new List<string>();
        private void ClearInstanceAssetData()
        {
            foreach (var kvp in instanceAssetDataDic)
            {
                if (!kvp.Value.IsInUsed())
                {
                    removedAssetAdress.Add(kvp.Key);
                }
            }

            removedAssetAdress.ForEach((key) =>
            {
                instanceAssetDataDic[key].Release();
                instanceAssetDataDic.Remove(key);
            });
            removedAssetAdress.Clear();
        }
        
        private void ClearAssetData()
        {
            foreach (var kvp in assetDataDic)
            {
                if (!kvp.Value.IsValid())
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

        private UnityObject GetObjectInstance(string address, AssetData assetData)
        {
            if (!instanceAssetDataDic.TryGetValue(address, out InstanceAssetData instance))
            {
                instance = new InstanceAssetData(assetData);
                instanceAssetDataDic.Add(address, instance);
            } 

            return instance.GetInstance();
        }
        
        public AssetHandle LoadAssetAsync(string address,
            OnAssetLoadFinishCallback finish,
            OnAssetLoadProgressCallback progress, SystemObject userData)
        {
            return LoadAssetsAsync(new string[] { address }, finish, progress, null, null,userData);
        }

        public AssetHandle LoadAssetsAsync(string[] addresses,
            OnAssetLoadFinishCallback singleFinish,
            OnAssetLoadProgressCallback singleProgress,
            OnAssetsLoadFinishCallback allFinish,
            OnAssetsLoadProgressCallback allProgress, SystemObject userData)
        {
            return LoadAsync(addresses, false, singleFinish, singleProgress, allFinish, allProgress, userData);
        }

        public AssetHandle LoadAssetsByLabeAsync(string label, 
            OnAssetLoadFinishCallback singleFinish,
            OnAssetLoadProgressCallback singleProgress,
            OnAssetsLoadFinishCallback allFinish,
            OnAssetsLoadProgressCallback allProgress, SystemObject userData)
        {
            return LoadByLabelAsync(label, false, singleFinish, singleProgress, allFinish, allProgress, userData);
        }

        public AssetHandle InstanceAssetAsync(string address,
            OnAssetLoadFinishCallback finish,
            OnAssetLoadProgressCallback progress, SystemObject userData)
        {
            return InstanceAssetsAsync(new string[] { address }, finish, progress, null, null, userData);
        }

        public AssetHandle InstanceAssetsAsync(string[] addresses,
             OnAssetLoadFinishCallback singleFinish,
            OnAssetLoadProgressCallback singleProgress,
            OnAssetsLoadFinishCallback allFinish,
            OnAssetsLoadProgressCallback allProgress, SystemObject userData)
        {
            return LoadAsync(addresses, true, singleFinish, singleProgress, allFinish, allProgress, userData);
        }

        public AssetHandle InstanceAssetsByLabelAsync(string label,
            OnAssetLoadFinishCallback singleFinish,
            OnAssetLoadProgressCallback singleProgress,
            OnAssetsLoadFinishCallback allFinish,
            OnAssetsLoadProgressCallback allProgress, SystemObject userData)
        {
            return LoadByLabelAsync(label, true,singleFinish, singleProgress, allFinish, allProgress, userData);
        }

        private AssetHandle LoadAsync(string[] addresses, bool isInstance,
            OnAssetLoadFinishCallback singleFinish,
            OnAssetLoadProgressCallback singleProgress,
            OnAssetsLoadFinishCallback allFinish,
            OnAssetsLoadProgressCallback allProgress, SystemObject userData)
        {
            LoadData loadData = new LoadData();
            loadData.uniqueID = idCreator.Next();
            loadData.Addresses = addresses;
            loadData.isInstance = isInstance;

            loadData.singleFinish = singleFinish;
            loadData.singleProgress = singleProgress;
            loadData.allFinish = allFinish;
            loadData.allProgress = allProgress;

            loadData.userData = userData;

            loadDataORM.PushData(loadData);

            foreach (var address in addresses)
            {
                RetainOrCreatAssetData(address);
            }

            return new AssetHandle() { uniqueID = loadData.uniqueID, addresses = addresses, releaseAction = ReleaseAsset };
        }

        private AssetHandle LoadByLabelAsync(string label, bool isInstance,
            OnAssetLoadFinishCallback singleFinish,
            OnAssetLoadProgressCallback singleProgress,
            OnAssetsLoadFinishCallback allFinish,
            OnAssetsLoadProgressCallback allProgress,SystemObject userData)
        {
            LoadData loadData = new LoadData();
            loadData.uniqueID = idCreator.Next();
            loadData.isInstance = isInstance;

            loadData.singleFinish = singleFinish;
            loadData.singleProgress = singleProgress;
            loadData.allFinish = allFinish;
            loadData.allProgress = allProgress;

            loadData.userData = userData;

            loadDataORM.PushData(loadData);

            AssetHandle assetHandle = new AssetHandle() { uniqueID = loadData.uniqueID, releaseAction = ReleaseAsset };

            Addressables.LoadResourceLocationsAsync(label).Completed += (locations) =>
            {
                if (loadDataORM.Contain(loadData.uniqueID))
                {
                    if (locations.Status == AsyncOperationStatus.Succeeded)
                    {
                        string[] addresses = new string[locations.Result.Count];
                        for (int i = 0; i < locations.Result.Count; ++i)
                        {
                            addresses[i] = locations.Result[i].PrimaryKey;
                            RetainOrCreatAssetData(addresses[i]);
                        }
                        loadData.Addresses = addresses;
                        assetHandle.addresses = addresses;
                    }
                    else
                    {
                        loadDataORM.DeleteByData(loadData);
                        assetHandle.IsValid = false;
                        allFinish?.Invoke(null, null,userData);
                    }
                }

                Addressables.Release(locations);
            };

            return assetHandle;
        }

        private void RetainOrCreatAssetData(string address)
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

        public void ReleaseAsset(AssetHandle assetHandle)
        {
            if(!assetHandle.IsValid)
            {
                return;
            }
            

            if (loadDataORM.Contain(assetHandle.uniqueID))
            {
                LoadData data = loadDataORM.GetDataByKey(assetHandle.uniqueID);
                ReleaseAssetByLoadData(data);
            }
            else
            {
                if (!assetHandle.isInstance && assetHandle.addresses != null)
                {
                    foreach (var address in assetHandle.addresses)
                    {
                        AssetData assetData = assetDataDic[address];
                        assetData.ReleaseRefCount();
                    }
                }
            }

            assetHandle.IsValid = false;
        }

        private void ReleaseAssetByLoadData(LoadData loadData)
        {
            loadDataORM.DeleteByData(loadData);

            if (loadData.Addresses == null || loadData.Addresses.Length==0)
            {
                return;
            }

            for (int i = 0; i < loadData.Addresses.Length; i++)
            {
                AssetData assetData = assetDataDic[loadData.Addresses[i]];

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
                if(handle.Equals(assetData.Handle))
                {
                    assetData.Handle.Completed -= OnAssetLoadComplete;
                    assetData.Status = AssetDataStatus.Loaded;
                    break;
                }
            }
        }

        private void CheckUnloadOperation()
        {
            if (unloadAsyncOper != null)
            {
                if (unloadAsyncOper.isDone)
                {
                    unloadAsyncOper = null;
                    GC.Collect();

                    unloadCallback?.Invoke();
                }
            }
        }

        private AsyncOperation unloadAsyncOper = null;
        private Action unloadCallback;
        public void UnloadUnsedAssets(Action callback)
        {
            if (unloadAsyncOper == null)
            {
                GC.Collect();
                unloadAsyncOper = Resources.UnloadUnusedAssets();

                unloadCallback = callback;
            }
        }

        public T Instantiate<T>(string address) where T : UnityObject
        {
            if(instanceAssetDataDic.TryGetValue(address,out InstanceAssetData instanceAssetData))
            {
                return (T)instanceAssetData.GetInstance();
            }

            return null;
        }
    }
    
    

}
