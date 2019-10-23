using Dot.Core.Loader;
using Dot.Core.Timer;
using Dot.Core.Util;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Pool
{
    public delegate void OnPoolComplete(string spawnName, string assetPath);

    /// <summary>
    /// 对于需要Pool异步加载的资源，可以通过PoolData指定对应池的属性，等到资源加载完成将会使用指定的属性设置缓存池
    /// </summary>
    public class PoolData
    {
        public string spawnName;
        public string assetPath;
        public bool isAutoClean = true;

        public int preloadTotalAmount = 0;
        public int preloadOnceAmount = 1;
        public OnPoolComplete completeCallback = null;

        public bool isCull = false;
        public int cullOnceAmount = 0;
        public int cullDelayTime = 30;

        public int limitMaxAmount = 0;
        public int limitMinAmount = 0;

        internal AssetLoaderHandle handle = null;
    }
    
    public class PoolManager : Util.Singleton<PoolManager>
    {
        private Transform cachedTransform = null;
        private Dictionary<string, SpawnPool> spawnDic = new Dictionary<string, SpawnPool>();

        private float cullTimeInterval = 60f;
        private TimerTaskInfo cullTimerTask = null;

        private List<PoolData> poolDatas = new List<PoolData>();

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
                spawn.DestroySpawn();
                spawnDic.Remove(name);
            }
        }

        /// <summary>
        /// 使用PoolData进行资源加载，资源加载完成后创建对应的缓存池
        /// </summary>
        /// <param name="poolData"></param>
        public void LoadAssetToCreateGameObjectPool(PoolData poolData)
        {
            SpawnPool spawnPool = GetSpawnPool(poolData.spawnName);
            if(spawnPool == null)
            {
                CreateSpawnPool(poolData.spawnName);
            }else
            {
                if(spawnPool.HasGameObjectPool(poolData.assetPath))
                {
                    Debug.LogWarning("PoolManager::LoadAssetToCreateGameObjectPool->GameObjectPool has been created!");
                    return;
                }
            }

            for(int i =0;i< poolDatas.Count;i++)
            {
                PoolData pData = poolDatas[i];
                if(pData.spawnName == poolData.spawnName && pData.assetPath == poolData.assetPath)
                {
                    Debug.LogError("PoolManager::CreateGameObjectPool->pool data has been added");
                    return;
                }
            }

            AssetLoaderHandle assetHandle = Loader.AssetManager.GetInstance().LoadAssetAsync(poolData.assetPath, OnLoadComplete, AssetLoaderPriority.Default,null, poolData);
            poolData.handle = assetHandle;
            poolDatas.Add(poolData);
        }

        private void OnLoadComplete(string assetPath,UnityObject uObj,System.Object userData)
        {
            PoolData poolData = userData as PoolData;

            if(!poolDatas.Contains(poolData))
            {
                return;
            }

            poolDatas.Remove(poolData);
            
            if(uObj is GameObject templateGO)
            {
                SpawnPool spawnPool = GetSpawnPool(poolData.spawnName);
                if(spawnPool!=null)
                {
                    GameObjectPool objPool = spawnPool.CreateGameObjectPool(poolData.assetPath, templateGO);
                    objPool.isAutoClean = poolData.isAutoClean;
                    objPool.preloadTotalAmount = poolData.preloadTotalAmount;
                    objPool.preloadOnceAmount = poolData.preloadOnceAmount;
                    objPool.completeCallback = poolData.completeCallback;
                    objPool.isCull = poolData.isCull;
                    objPool.cullOnceAmount = poolData.cullOnceAmount;
                    objPool.cullDelayTime = poolData.cullDelayTime;
                    objPool.limitMaxAmount = poolData.limitMaxAmount;
                    objPool.limitMinAmount = poolData.limitMinAmount;
                }
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
                kvp.Value.DestroySpawn();
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
       
    }
}
