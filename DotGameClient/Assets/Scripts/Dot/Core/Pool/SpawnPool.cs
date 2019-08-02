using Dot.Core.Logger;
using System.Collections.Generic;
using UnityEngine;

namespace Dot.Core.Pool
{
    public class SpawnPool
    {
        private Dictionary<string, GameObjectPool> goPools = new Dictionary<string, GameObjectPool>();
        internal Transform CachedTransform { get; private set; } = null;
        internal string PoolName { get; private set; } = string.Empty;

        internal SpawnPool()
        {
        }

        internal void InitSpawn(string pName, Transform parentTran)
        {
            PoolName = pName;

            CachedTransform = new GameObject($"Spawn_{PoolName}").transform;
            CachedTransform.SetParent(parentTran, false);
        }
        
        public GameObjectPool GetGameObjectPool(string assetPath)
        {
            if(goPools.TryGetValue(assetPath,out GameObjectPool goPool))
            {
                return goPool;
            }
            return null;
        }

        public GameObjectPool CreateGameObjectPool(string assetPath,GameObject template)
        {
            if(template == null)
            {
                DebugLogger.LogError("SpawnPool::CreateGameObjectPool->Template Item is Null");
                return null;
            }
            if (goPools.TryGetValue(assetPath, out GameObjectPool goPool))
            {
                DebugLogger.LogWarning("SpawnPool::CreateGameObjectPool->the asset pool has been created.assetPath = " + assetPath);
            }
            else
            {
                goPool = new GameObjectPool();
                goPool.InitPool(this, assetPath, template);

                goPools.Add(assetPath, goPool);
            }
           return goPool;
        }

        public void DeleteGameObjectPool(string assetPath)
        {
            GameObjectPool gObjPool = GetGameObjectPool(assetPath);
            if(gObjPool!=null)
            {
                gObjPool.ClearPool(true);
            }
        }

        internal void CullSpawn(float deltaTime)
        {
            foreach(var kvp in goPools)
            {
                if(kvp.Value.isCull)
                {
                    kvp.Value.CullPool(deltaTime);
                }
            }
        }

        public void ClearPoolByAssetPath(string aPath,bool isForce = false)
        {
            if (goPools.TryGetValue(aPath, out GameObjectPool goPool))
            {
                if(goPool.ClearPool(isForce))
                {
                    goPools.Remove(aPath);
                }else
                {
                    DebugLogger.LogError("SpawnPool::ClearPoolByAssetPath->the pool of assetPath can't be clear.AssetPath = " + aPath);
                }
            }
        }

        internal void ClearPool(bool isForce = false)
        {
            List<string> clearPoolNames = new List<string>();
            foreach(var kvp in goPools)
            {
                if(kvp.Value.ClearPool(isForce))
                {
                    clearPoolNames.Add(kvp.Key);
                }
            }
            foreach(var name in clearPoolNames)
            {
                goPools.Remove(name);
            }
            if(goPools.Count == 0)
            {
                UnityEngine.Object.Destroy(CachedTransform.gameObject);
            }
        }


#if UNITY_EDITOR
        public string[] GetAssetPaths()
        {
            string[] result = new string[goPools.Count];
            int i = 0;
            foreach (var key in goPools.Keys)
            {
                result[i] = key;
                ++i;
            }
            System.Array.Sort(result);
            return result;
        }
#endif
    }
}
