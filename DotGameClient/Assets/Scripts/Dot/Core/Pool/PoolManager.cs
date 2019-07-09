using Dot.Core.Logger;
using Dot.Core.Manager;
using Dot.Core.Timer;
using Dot.Core.Util;
using System.Collections.Generic;
using UnityEngine;

namespace Dot.Core.Pool
{
    public class PoolManager : IGlobalManager
    {
        private Transform cachedTransform = null;
        private Dictionary<string, SpawnPool> spawnPools = new Dictionary<string, SpawnPool>();

        private TimerTaskInfo cullTimerTask = null;
        public float cullTimeInterval = 30f;

        public PoolManager(){}

        public int Priority { get; set; }

        public void DoInit()
        {
            cachedTransform = DontDestoryHandler.CreateTransform("PoolManager");
            cullTimerTask = GlobalManager.GetInstance().TimerMgr.AddTimerTask(cullTimeInterval, 0, null, OnCullTimerUpdate, null, null);
        }

        public void DoReset()
        {
            foreach (var kvp in spawnPools)
            {
                kvp.Value.ClearPool(true);
            }
            spawnPools.Clear();
        }

#if UNITY_EDITOR
        public string[] GetSpawnPoolNames()
        {
            string[] names = new string[spawnPools.Count];
            int i = 0;
            foreach(var key in spawnPools.Keys)
            {
                names[i] = key;
                ++i;
            }

            System.Array.Sort(names);
            return names;
        }
#endif

        public SpawnPool GetSpawnPool(string name,bool createIfNot = true)
        {
            if (spawnPools.TryGetValue(name, out SpawnPool pool))
            {
                return pool;
            }
            else
            {
                if (createIfNot)
                {
                    pool = new SpawnPool(name, cachedTransform);
                    spawnPools.Add(name, pool);
                }
            }
            return pool;
        }

        public GameObjectPool CreateGameObjectPool(string poolName,string assetPath, GameObjectPoolItem tempItem)
        {
            SpawnPool sp = GetSpawnPool(poolName, true);
            return sp.CreateGameObjectPool(assetPath, tempItem);
        }

        public GameObjectPoolItem GetPoolItemFromPool(string poolName,string assetPath,bool isAutoSetActive = true)
        {
            SpawnPool sp = GetSpawnPool(poolName, false);
            if(sp == null)
            {
                DebugLogger.LogError("PoolManager::GetPoolItemFromPool->SpawnPool not create.PoolName = " + poolName + ", assetPath = " + assetPath);
                return null;
            }
            GameObjectPool goPool = sp.GetGameObjectPool(assetPath);
            if(goPool == null)
            {
                DebugLogger.LogError("PoolManager::GetPoolItemFromPool->GameObjectPool Is Null.PoolName = " + poolName + ", assetPath = " + assetPath);
                return null;
            }

            return goPool.GetPoolItem(isAutoSetActive);
        }

        public GameObject GetGameObjectFromPool(string poolName, string assetPath, bool isAutoActive = true)
        {
            var item = GetPoolItemFromPool(poolName, assetPath, isAutoActive);
            if(item)
            {
                return item.CachedGameObject;
            }
            return null;
        }

        public void ReleasePoolItemToPool(GameObjectPoolItem item)
        {
            SpawnPool sp = GetSpawnPool(item.PoolName, false);
            if (sp == null)
            {
                DebugLogger.LogError("PoolManager::ReleaseItemToGameObjectPool->SpawnPool not create.PoolName = " + item.PoolName + ", assetPath = " + item.AssetPath);
                return;
            }
            GameObjectPool goPool = sp.GetGameObjectPool(item.AssetPath);
            if (goPool == null)
            {
                DebugLogger.LogError("PoolManager::ReleaseItemToGameObjectPool->GameObjectPool Is Null.PoolName = " + item.PoolName + ", assetPath = " + item.AssetPath);
                return;
            }
            goPool.ReleasePoolItem(item);
        }

        public void ReleaseGameObjectToPool(GameObject go)
        {
            GameObjectPoolItem pItem = go.GetComponent<GameObjectPoolItem>();
            if (pItem == null)
            {
                DebugLogger.LogError("PoolManager::ReleaseGameObjectToPool->poolItem is null");
                return;
            }
            ReleasePoolItemToPool(pItem);
        }

        private void OnCullTimerUpdate(System.Object obj)
        {
            foreach(var kvp in spawnPools)
            {
                kvp.Value.CullPool(cullTimeInterval);
            }
        }

        public void DoDispose()
        {
            DoReset();
            if(cullTimerTask != null)
            {
                GlobalManager.GetInstance().TimerMgr.RemoveTimerTask(cullTimerTask);
            }
            cullTimerTask = null;
            spawnPools = null;
        }
    }
}
