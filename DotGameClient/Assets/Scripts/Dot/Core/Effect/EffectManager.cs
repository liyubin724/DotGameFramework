using Dot.Core.Logger;
using Dot.Core.Pool;
using Dot.Core.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dot.Core.Effect
{
    public enum EffectScenarioType
    {
        UI,
        Timline,
    }

    public class EffectManager : Util.Singleton<EffectManager>
    {
        private readonly static string ROOT_NAME = "Effect Root";
        private readonly static string CONTROLLER_SPAWN_NAME = "EffectControllerSpawn";
        private readonly static string CONTROLLER_POOL_PATH = "effect_controller_virtual_path";

        private Transform rootTransform = null;
        private GameObjectPool effectControllerPool = null;
        private Dictionary<EffectScenarioType, string> scenarioSpawnDic = new Dictionary<EffectScenarioType, string>(); 

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
            effectControllerPool.completeCallback = OnInitComplete;
        }

        public void SetScenarioSpawnName(EffectScenarioType scenarioType,string spawnName)
        {
            if(scenarioSpawnDic.ContainsKey(scenarioType))
            {
                DebugLogger.LogError("");
                return;
            }
            scenarioSpawnDic.Add(scenarioType, spawnName);
        }

        public void CleanSpawnPool(string spawnName)
        {
            var keys = scenarioSpawnDic.Keys;
            foreach(var key in keys)
            {
                if(scenarioSpawnDic[key] == spawnName)
                {
                    scenarioSpawnDic.Remove(key);
                }
            }

            PoolManager.GetInstance().DeleteSpawnPool(spawnName);
        }

        /// <summary>
        /// 进行特效的预加载，并创建缓存池
        /// </summary>
        /// <param name="poolData"></param>
        public void PreloadEffect(PoolData poolData)
        {
            PoolManager.GetInstance().CreateGameObjectPool(poolData);
        }
        
        public void PreloadEffect(string spawnName, string assetPath, int preloadCount, OnPoolComplete callback)
        {
            PoolData poolData = new PoolData()
            {
                spawnName = spawnName,
                assetPath = assetPath,
                preloadTotalAmount = preloadCount,
                completeCallback = callback,
            };
            PreloadEffect(poolData);
        }
        /// <summary>
        /// 使用指定的资源的路径（地址）创建特效
        /// 使用此接口创建的特效不会加入到缓存池中，释放时会直接删除
        /// 可以通过指定isAutoRelease=true，来自动管理特效的生命周期
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="isAutoRelease"></param>
        /// <returns></returns>
        public EffectController GetEffect(string assetPath, bool isAutoRelease = true)
        {
            EffectController effectController = effectControllerPool.GetComponentItem<EffectController>(false);
            effectController.CachedTransform.SetParent(rootTransform, false);
            if(isAutoRelease)
                effectController.effectFinished += OnEffectComplete;

            effectController.SetEffect(assetPath);

            return effectController;
        }

        /// <summary>
        /// 获取指定的特效，并创建其对应的缓存池,特效释放后会被回收到池中
        /// 可以通过指定isAutoRelease=true，来自动管理特效的生命周期
        /// </summary>
        /// <param name="spawnName"></param>
        /// <param name="assetPath"></param>
        /// <param name="isAutoRelease"></param>
        /// <returns></returns>
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

        public EffectController GetEffect(string assetPath,EffectScenarioType scenarioType, bool isAutoRelease = true)
        {
            if(scenarioSpawnDic.TryGetValue(scenarioType,out string spawnName))
            {
                return GetEffect(spawnName, assetPath, isAutoRelease);
            }else
            {
                return GetEffect(assetPath, isAutoRelease);
            }
        }

        /// <summary>
        /// 释放指定的特效
        /// </summary>
        /// <param name="effect"></param>
        public void ReleaseEffect(EffectController effect)
        {
            effect.Stop();
            effect.GetEffect()?.ReleaseItem();
            effect.ReleaseItem();
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
