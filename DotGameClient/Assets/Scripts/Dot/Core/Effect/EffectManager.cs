using Dot.Core.Pool;
using Dot.Core.Util;
using System.Collections.Generic;

namespace Dot.Core.Effect
{
    public class EffectManager : Singleton<EffectManager>
    {
        private Dictionary<string, string> assetSpawnCached = new Dictionary<string, string>();
        private List<EffectBehaviour> effectList = new List<EffectBehaviour>();

        public void PreloadEffect(string spawnName, string assetPath, int preloadCount, OnPoolPreloadComplete callback)
        {
            PoolData poolData = new PoolData()
            {
                spawnName = spawnName,
                assetPath = assetPath,
                preloadTotalAmount = preloadCount,
                preloadCompleteCallback = callback,
            };
            assetSpawnCached.Add(assetPath, spawnName);
            PoolManager.GetInstance().CreateGameObjectPool(poolData);
        }

        public EffectBehaviour GetEffect(string assetPath)
        {
            EffectBehaviour effect = null;
            if (assetSpawnCached.TryGetValue(assetPath,out string spawnName))
            {
                if(PoolManager.GetInstance().HasSpawnPool(spawnName))
                {
                    SpawnPool spawnPool = PoolManager.GetInstance().GetOrCreateSpawnPool(spawnName);
                    GameObjectPool objPool = spawnPool.GetGameObjectPool(assetPath);
                    if(objPool!=null)
                    {
                        GameObjectPoolItem effectItem = objPool.GetPoolItem();
                        effect = effectItem as EffectBehaviour;
                        if(effect == null)
                        {
                            objPool.ReleasePoolItem(effectItem);
                        }
                    }
                }
                else
                {
                    assetSpawnCached.Remove(assetPath);
                }
            }

            if(effect!=null)
            {
                effectList.Add(effect);
                effect.effectDeadCallback += OnEffectDead;
            }

            return effect;
        }

        public void ReleaseEffect(EffectBehaviour effect)
        {
            effectList.Remove(effect);
            effect.ReleaseItem();
        }

        public void CleanSpawnPool(string spawnName)
        {
            List<string> assetPaths = new List<string>();
            foreach(var kvp in assetSpawnCached)
            {
                if(kvp.Value == spawnName)
                {
                    assetPaths.Add(kvp.Key);
                }
            }

            assetPaths.ForEach((assetPath) =>
            {
                assetSpawnCached.Remove(assetPath);
            });
            PoolManager.GetInstance().DeleteSpawnPool(spawnName);
        }

        public void CleanObjectPool(string assetPath)
        {
            if (assetSpawnCached.TryGetValue(assetPath, out string spawnName))
            {
                PoolManager.GetInstance().DeleteGameObjectPool(spawnName, assetPath);
            }
            
            assetSpawnCached.Remove(assetPath);
        }

        private void OnEffectDead(EffectBehaviour effect)
        {
            ReleaseEffect(effect);
        }

        
    }
}
