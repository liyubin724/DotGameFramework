using Dot.Core.Asset;
using Dot.Core.Logger;
using Dot.Core.Timer;
using Dot.Core.Util;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Pool
{
    public delegate void OnPoolPreloadComplete(string spawnName, string assetPath);

    public class PoolData
    {
        public string spawnName;
        public string assetPath;
        public bool isAutoClean = true;

        public int preloadTotalAmount = 0;
        public int preloadOnceAmout = 1;
        public OnPoolPreloadComplete preloadCompleteCallback = null;

        public bool isCull = false;
        public int cullOnceAmout = 0;
        public int cullDelayTime = 30;

        public int limitMaxAmount = 0;
        public int limitMinAmount = 0;
    }
    
    public class PoolManager : Singleton<PoolManager>
    {
        private Transform cachedTransform = null;
        private Dictionary<string, SpawnPool> spawnDic = new Dictionary<string, SpawnPool>();

        private float cullTimeInterval = 30f;
        private TimerTaskInfo cullTimerTask = null;

        private Dictionary<AssetHandle, PoolData> poolDataDic = new Dictionary<AssetHandle, PoolData>();

        protected override void DoInit()
        {
            cachedTransform = DontDestroyHandler.CreateTransform("PoolManager");
            cullTimerTask = GameApplication.GTimer.AddTimerTask(cullTimeInterval, 0, null, OnCullTimerUpdate, null, null);
        }
        
        public bool HasSpawnPool(string name)=> spawnDic.ContainsKey(name);
        
        public SpawnPool GetOrCreateSpawnPool(string name)
        {
            if (spawnDic.TryGetValue(name, out SpawnPool pool))
            {
                return pool;
            }
            else
            {
                pool = new SpawnPool();
                pool.InitSpawn(name, cachedTransform);

                spawnDic.Add(name, pool);
            }
            return pool;
        }

        public void DeleteSpawnPool(string name)
        {
            if (spawnDic.TryGetValue(name, out SpawnPool spawn))
            {
                spawn.ClearPool(true);
                spawnDic.Remove(name);
            }
        }

        public void DeleteGameObjectPool(string spawnName,string assetPath)
        {
            foreach (var kvp in poolDataDic)
            {
                if (kvp.Value.assetPath == assetPath && kvp.Value.spawnName == spawnName)
                {
                    poolDataDic.Remove(kvp.Key);
                    return;
                }
            }

            if(HasSpawnPool(spawnName))
            {
                SpawnPool spawnPool = GetOrCreateSpawnPool(spawnName);
                spawnPool.DeleteGameObjectPool(assetPath);
            }
        }

        public void ReleaseGameObjectToPool(GameObject go)
        {
            GameObjectPoolItem pItem = go.GetComponent<GameObjectPoolItem>();
            if (pItem == null)
            {
                DebugLogger.LogError("PoolManager::ReleaseGameObjectToPool->poolItem is null");

                UnityObject.Destroy(go);
                return;
            }
            pItem.ReleaseItem();
        }

        public void CreateGameObjectPool(PoolData poolData)
        {
            foreach(var kvp in poolDataDic)
            {
                if(kvp.Value.assetPath == poolData.assetPath)
                {
                    DebugLogger.LogError("");
                    return;
                }
            }

            AssetHandle assetHandle = AssetLoader.GetInstance().InstanceAssetAsync(poolData.assetPath, OnLoadComplete, null,null);
            poolDataDic.Add(assetHandle, poolData);
        }

        private void OnLoadComplete(string assetPath,UnityObject uObj,System.Object userData)
        {
            if (uObj == null)
            {
                DebugLogger.LogError("");
                return;
            }

            PoolData poolData = null;
            foreach(var kvp in poolDataDic)
            {
                if(kvp.Key.Address == assetPath)
                {
                    poolData = kvp.Value;
                    poolDataDic.Remove(kvp.Key);
                    break;
                }
            }

            if(poolData == null)
            {
                UnityObject.Destroy(uObj);
                return;
            }

            if(uObj is GameObject templateGO)
            {
                SpawnPool spawnPool = GetOrCreateSpawnPool(poolData.spawnName);
                GameObjectPool objPool = spawnPool.CreateGameObjectPool(poolData.assetPath, templateGO);
                objPool.isAutoClean = poolData.isAutoClean;
                objPool.preloadTotalAmount = poolData.preloadTotalAmount;
                objPool.preloadOnceAmout = poolData.preloadOnceAmout;
                objPool.preloadCompleteCallback = poolData.preloadCompleteCallback;
                objPool.isCull = poolData.isCull;
                objPool.cullOnceAmout = poolData.cullOnceAmout;
                objPool.cullDelayTime = poolData.cullDelayTime;
                objPool.limitMaxAmount = poolData.limitMaxAmount;
                objPool.limitMinAmount = poolData.limitMinAmount;
            }else
            {
                UnityObject.Destroy(uObj);
                return;
            }
        }

        private void OnCullTimerUpdate(System.Object obj)
        {
            foreach(var kvp in spawnDic)
            {
                kvp.Value.CullSpawn(cullTimeInterval);
            }
        }
        
        public override void DoReset()
        {
            foreach (var kvp in spawnDic)
            {
                kvp.Value.ClearPool(true);
            }
            spawnDic.Clear();
        }

        public override void DoDispose()
        {
            DoReset();
            if(cullTimerTask != null)
            {
                GameApplication.GTimer.RemoveTimerTask(cullTimerTask);
            }
            cullTimerTask = null;
            spawnDic = null;
        }
        
#if UNITY_EDITOR
        public string[] GetSpawnPoolNames()
        {
            string[] names = new string[spawnDic.Count];
            int i = 0;
            foreach (var key in spawnDic.Keys)
            {
                names[i] = key;
                ++i;
            }

            System.Array.Sort(names);
            return names;
        }
#endif
    }
}
