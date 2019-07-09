using Dot.Core.Logger;
using System.Collections.Generic;
using UnityEngine;

namespace Dot.Core.Pool
{
    public class SpawnPool
    {
        private Dictionary<string, GameObjectPool> goPools = new Dictionary<string, GameObjectPool>();

        public Transform CachedTransform { get; private set; } = null;
        public string PoolName { get; private set; } = string.Empty;

        internal SpawnPool(string pName,Transform parentTran)
        {
            PoolName = pName;
            GameObject go = new GameObject("SP_" + PoolName);
            CachedTransform = go.transform;
            CachedTransform.SetParent(parentTran, false);
        }

#if UNITY_EDITOR
        public string[] GetAssetPaths()
        {
            string[] result = new string[goPools.Count];
            int i = 0;
            foreach(var key in goPools.Keys)
            {
                result[i] = key;
                ++i;
            }
            System.Array.Sort(result);
            return result;
        }
#endif

        internal GameObjectPool GetGameObjectPool(string assetPath)
        {
            if(goPools.TryGetValue(assetPath,out GameObjectPool goPool))
            {
                return goPool;
            }
            return null;
        }

        internal GameObjectPool CreateGameObjectPool(string assetPath,GameObjectPoolItem tempItem)
        {
            if(tempItem == null)
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
                goPool = new GameObjectPool(this,assetPath, tempItem);
                goPools.Add(assetPath, goPool);
            }
           return goPool;
        }

        internal void CullPool(float deltaTime)
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
        }
    }
}
