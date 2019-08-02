using Dot.Core.Pool;
using Dot.Core.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dot.Core.Effect
{
    public class EffectManager : Singleton<EffectManager>
    {
        private readonly static string EFFECT_CONTROLLER_SPAWN_NAME = "EffectControllerSpawn";
        private readonly static string EFFECT_CONTROLLER_POOL_PATH = "effect_controller_virtual_path";

        private GameObjectPool effectControllerPool = null;

        public Action initFinishCallback;

        protected override void DoInit()
        {
            SpawnPool spawnPool = PoolManager.GetInstance().GetOrCreateSpawnPool(EFFECT_CONTROLLER_SPAWN_NAME);

            effectControllerPool = spawnPool.CreateGameObjectPool(EFFECT_CONTROLLER_POOL_PATH, GetEffectControllerTemplate());
            effectControllerPool.isUsedTemplateForNewItem = true;
            effectControllerPool.isAutoClean = false;
            effectControllerPool.preloadTotalAmount = 20;
            effectControllerPool.preloadOnceAmout = 2;
            effectControllerPool.preloadCompleteCallback = OnInitComplete;
        }
        
        public void PreloadEffect(string spawnName, string assetPath, int preloadCount, OnPoolPreloadComplete callback)
        {
            PoolData poolData = new PoolData()
            {
                spawnName = spawnName,
                assetPath = assetPath,
                preloadTotalAmount = preloadCount,
                preloadCompleteCallback = callback,
            };
            PoolManager.GetInstance().CreateGameObjectPool(poolData);
        }

        public EffectController GetEffect(string assetPath)
        {
            EffectController effectController = effectControllerPool.GetComponentItem<EffectController>(false);
            effectController.SetEffect(assetPath);

            return effectController;
        }

        public EffectController GetEffect(string spawnName, string assetPath)
        {
            EffectController effectController = effectControllerPool.GetComponentItem<EffectController>(false);
            
            SpawnPool spawnPool = PoolManager.GetInstance().GetOrCreateSpawnPool(spawnName);
            GameObjectPool objPool = spawnPool.GetGameObjectPool(assetPath);
            if(objPool != null)
            {
                EffectBehaviour effectItem = objPool.GetComponentItem<EffectBehaviour>(false);
                if (effectItem == null)
                {
                    Debug.LogError("EffectManager::GetEffect->effectItem is Null,it should be EffectBehaviour");
                    objPool.ReleasePoolItem(effectItem);

                    effectController.ReleaseItem();
                    return null;
                }
                else
                {
                    effectController.SetEffect(effectItem);
                }
            }else
            {
                effectController.SetEffect(assetPath, spawnName);
            }

            effectController.effectFinished += OnEffectComplete;
            return effectController;
        }
        
        public void ReleaseEffect(EffectController effect)
        {
            effect.Stop();
            EffectBehaviour effectBehaviour = effect.GetEffect();
            if (effectBehaviour != null)
            {
                if(!string.IsNullOrEmpty(effectBehaviour.SpawnName) && !string.IsNullOrEmpty(effectBehaviour.AssetPath))
                {
                    SpawnPool spawnPool = PoolManager.GetInstance().GetOrCreateSpawnPool(effectBehaviour.SpawnName);
                    GameObjectPool goPool = spawnPool.GetGameObjectPool(effectBehaviour.AssetPath);
                    if (goPool == null)
                    {
                        effectBehaviour.DoDespawned();
                        goPool = spawnPool.CreateGameObjectPool(effectBehaviour.AssetPath, effectBehaviour.gameObject);
                    }
                    else
                    {
                        goPool.ReleasePoolItem(effectBehaviour);
                    }
                }
                else
                {
                    effectBehaviour.ReleaseItem();
                }
            }

            effect.ReleaseItem();
        }

        public void CleanSpawnPool(string spawnName)
        {
            PoolManager.GetInstance().DeleteSpawnPool(spawnName);
        }
        
        private void OnEffectComplete(EffectController effect)
        {
            effect.effectFinished -= OnEffectComplete;
        }

        private void OnInitComplete(string spawnName, string assetPath)
        {
            initFinishCallback?.Invoke();
            initFinishCallback = null;
        }

        private GameObject GetEffectControllerTemplate()
        {
            GameObject templateGO = new GameObject("Effect Controller");
            templateGO.AddComponent<EffectController>();

            return templateGO;
        }
    }
}
