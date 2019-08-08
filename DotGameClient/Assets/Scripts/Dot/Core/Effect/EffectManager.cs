﻿using Dot.Core.Pool;
using Dot.Core.Util;
using System;
using UnityEngine;

namespace Dot.Core.Effect
{
    public class EffectManager : Singleton<EffectManager>
    {
        private readonly static string ROOT_NAME = "Effect Root";
        private readonly static string CONTROLLER_SPAWN_NAME = "EffectControllerSpawn";
        private readonly static string CONTROLLER_POOL_PATH = "effect_controller_virtual_path";

        private Transform rootTransform = null;
        private GameObjectPool effectControllerPool = null;

        public Action initFinishCallback;

        protected override void DoInit()
        {
            rootTransform = DontDestroyHandler.CreateTransform(ROOT_NAME);

            SpawnPool spawnPool = PoolManager.GetInstance().GetSpawnPool(CONTROLLER_SPAWN_NAME,true);

            effectControllerPool = spawnPool.CreateGameObjectPool(CONTROLLER_POOL_PATH, GetEffectControllerTemplate());
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

        public EffectController GetEffect(string assetPath, bool isAutoRelease = true)
        {
            EffectController effectController = effectControllerPool.GetComponentItem<EffectController>(false);
            effectController.CachedTransform.SetParent(rootTransform, false);
            if(isAutoRelease)
                effectController.effectFinished += OnEffectComplete;

            effectController.SetEffect(assetPath);

            return effectController;
        }

        public EffectController GetEffect(string spawnName, string assetPath,bool isAutoRelease = true)
        {
            EffectController effectController = effectControllerPool.GetComponentItem<EffectController>(false);
            effectController.CachedTransform.SetParent(rootTransform, false);
            if (isAutoRelease)
                effectController.effectFinished += OnEffectComplete;

            SpawnPool spawnPool = PoolManager.GetInstance().GetSpawnPool(spawnName,true);
            GameObjectPool objPool = spawnPool.GetGameObjectPool(assetPath);
            if(objPool != null)
            {
                EffectBehaviour effectItem = objPool.GetComponentItem<EffectBehaviour>(false);
                if(effectItem!=null)
                {
                    effectController.SetEffect(effectItem);
                }else
                {
                    Debug.LogError("EffectManager::GetEffect->effectItem is Null,it should be EffectBehaviour");
                }
            }else
            {
                effectController.SetEffect(assetPath, spawnName);
            }
            
            return effectController;
        }
        
        public void ReleaseEffect(EffectController effect)
        {
            effect.Stop();
            effect.GetEffect()?.ReleaseItem();
            effect.ReleaseItem();
        }

        public void CleanSpawnPool(string spawnName)
        {
            PoolManager.GetInstance().DeleteSpawnPool(spawnName);
        }
        
        private void OnEffectComplete(EffectController effect)
        {
            effect.effectFinished -= OnEffectComplete;
            ReleaseEffect(effect);
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
