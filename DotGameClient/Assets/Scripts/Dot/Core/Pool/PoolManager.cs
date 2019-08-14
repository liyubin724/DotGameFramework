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

    /// <summary>
    /// 对于需要Pool异步加载的资源，可以通过PoolData指定对应池的属性，等到资源加载完成将会使用指定的属性设置缓存池
    /// </summary>
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
    
    public class PoolManager : Util.Singleton<PoolManager>
    {
        private Transform cachedTransform = null;
        private Dictionary<string, SpawnPool> spawnDic = new Dictionary<string, SpawnPool>();

        private float cullTimeInterval = 30f;
        private TimerTaskInfo cullTimerTask = null;

        private Dictionary<AssetHandle, PoolData> poolDataDic = new Dictionary<AssetHandle, PoolData>();

        protected override void DoInit()
        {
            cachedTransform = DontDestroyHandler.CreateTransform("PoolManager");
            cullTimerTask = TimerManager.GetInstance().AddIntervalTimer(cullTimeInterval, OnCullTimerUpdate);
        }
        
        /// <summary>
        /// 判断是否存在指定的分组
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasSpawnPool(string name)=> spawnDic.ContainsKey(name);

        /// <summary>
        ///获取指定的分组，如果不存在可以指定isCreateIfNot为true进行创建
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isCreateIfNot"></param>
        /// <returns></returns>
        public SpawnPool GetSpawnPool(string name,bool isCreateIfNot = false)
        {
            if (spawnDic.TryGetValue(name, out SpawnPool pool))
            {
                return pool;
            }

            if(isCreateIfNot)
            {
                return CreateSpawnPool(name);
            }
            return null;
        }
        /// <summary>
        /// 创建指定名称的分组
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SpawnPool CreateSpawnPool(string name)
        {
            if (!spawnDic.TryGetValue(name, out SpawnPool pool))
            {
                pool = new SpawnPool();
                pool.InitSpawn(name, cachedTransform);

                spawnDic.Add(name, pool);
            }
            return pool;
        }
        /// <summary>
        /// 删除指定的分组，对应分组中所有的缓存池都将被删除
        /// </summary>
        /// <param name="name"></param>
        public void DeleteSpawnPool(string name)
        {
            if (spawnDic.TryGetValue(name, out SpawnPool spawn))
            {
                spawn.DestroyPool(true);
                spawnDic.Remove(name);
            }
        }
        /// <summary>
        /// 删除指定分组中的指定的缓存池
        /// </summary>
        /// <param name="spawnName"></param>
        /// <param name="assetPath"></param>
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
                SpawnPool spawnPool = GetSpawnPool(spawnName);
                spawnPool.DeleteGameObjectPool(assetPath);
            }
        }

        /// <summary>
        /// 将GameObject回收到缓存池中
        /// </summary>
        /// <param name="go"></param>
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

        /// <summary>
        /// 使用PoolData进行资源加载，资源加载完成后创建对应的缓存池
        /// </summary>
        /// <param name="poolData"></param>
        public void CreateGameObjectPool(PoolData poolData)
        {
            foreach(var kvp in poolDataDic)
            {
                if(kvp.Value.assetPath == poolData.assetPath && kvp.Value.spawnName == poolData.spawnName)
                {
                    DebugLogger.LogError("");
                    return;
                }
            }

            if(!HasSpawnPool(poolData.spawnName))
            {
                CreateSpawnPool(poolData.spawnName);
            }

            AssetHandle assetHandle = AssetLoader.GetInstance().InstanceAssetAsync(poolData.assetPath, OnLoadComplete, null, poolData);
            poolDataDic.Add(assetHandle, poolData);
        }

        private void OnLoadComplete(string assetPath,UnityObject uObj,System.Object userData)
        {
            PoolData poolData = userData as PoolData;
            AssetHandle assetHandle = null;
            foreach (var kvp in poolDataDic)
            {
                if(kvp.Value == poolData)
                {
                    assetHandle = kvp.Key;
                    break;
                }
            }
            poolDataDic.Remove(assetHandle);
            
            if(uObj is GameObject templateGO)
            {
                if(HasSpawnPool(poolData.spawnName))
                {
                    SpawnPool spawnPool = GetSpawnPool(poolData.spawnName);
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
                }
                else
                {
                    UnityObject.Destroy(uObj);
                }
                
            }else
            {
                UnityObject.Destroy(uObj);
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
                kvp.Value.DestroyPool(true);
            }
            spawnDic.Clear();
        }

        public override void DoDispose()
        {
            DoReset();
            if(cullTimerTask != null)
            {
                TimerManager.GetInstance().RemoveTimer(cullTimerTask);
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
