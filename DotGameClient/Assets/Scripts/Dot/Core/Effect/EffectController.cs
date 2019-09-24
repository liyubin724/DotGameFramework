using Dot.Core.Logger;
using Dot.Core.Pool;
using Dot.Core.Timer;
using UnityEngine;
using SystemObject = System.Object;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Effect
{
    public enum EffectStatus
    {
        None,
        Playing,
        Stopping,
        Dead,
    }

    public delegate void OnEffectFinish(EffectController effect);

    public class EffectController : GameObjectPoolItem
    {
        public bool isAutoPlayWhenEnable = false;
        public float lifeTime = 0.0f;
        public float stopDelayTime = 0.0f;

        public bool isMainPlayer = false;
        public event OnEffectFinish effectFinished = delegate(EffectController e) { };

        private EffectStatus status = EffectStatus.None;
        private TimerTaskInfo timer = null;
        //private AssetHandle effectAssetHandle = null;

        private void OnEnable()
        {
            if(isActiveAndEnabled)
            {
                Play();
            }
        }

        private EffectBehaviour effectBehaviour = null;
        internal EffectBehaviour GetEffect()
        {
            return effectBehaviour;
        }

        public void SetEffect(EffectBehaviour effect)
        {
            effectBehaviour = effect;
            effectBehaviour.CachedTransform.SetParent(CachedTransform, false);

            if(status == EffectStatus.Playing)
            {
                effectBehaviour.Play();
            }else if(status == EffectStatus.Stopping)
            {
                effectBehaviour.Stop();
            }else if(status == EffectStatus.Dead)
            {
                effectBehaviour.Dead();
            }
        }

        public void SetEffect(string effectPath, string spawnName=null)
        {
            //effectAssetHandle?.Release();
            //effectAssetHandle = AssetLoader.GetInstance().InstanceAssetAsync(effectPath, OnEffectLoadComplete, null, spawnName);
        }

        private void OnEffectLoadComplete(string effectPath,UnityObject uObj,SystemObject userData)
        {
            string spawnName = userData as string;
            //effectAssetHandle = null;
            GameObject effectGO = (GameObject)uObj;
            if(uObj == null)
            {
                DebugLogger.LogError($"EffectController::OnEffectLoadComplete->uObj is Null.assetPath = {effectPath}");
                return;
            }
            EffectBehaviour effectBehaviour = effectGO.GetComponent<EffectBehaviour>();
            if(effectBehaviour == null)
            {
                DebugLogger.LogError($"EffectController::OnEffectLoadComplete->Effect not contain EffectBehaviour.assetPath = {effectPath}");
                UnityObject.Destroy(uObj);
                return;
            }
            if(string.IsNullOrEmpty(spawnName))
            {
                SetEffect(effectBehaviour);
                return;
            }
            if (!PoolManager.GetInstance().HasSpawnPool(spawnName))
            {
                UnityObject.Destroy(uObj);
                return;
            }
            SpawnPool spawnPool = PoolManager.GetInstance().GetSpawnPool(spawnName);
            GameObjectPool objPool = spawnPool.GetGameObjectPool(effectPath);
            if(objPool == null)
            {
                objPool = spawnPool.CreateGameObjectPool(effectPath, effectGO);
            }
            effectBehaviour = objPool.GetComponentItem<EffectBehaviour>();
            if (effectBehaviour == null)
            {
                DebugLogger.LogError($"EffectController::OnEffectLoadComplete->Effect not contain EffectBehaviour.assetPath = {effectPath}");
                UnityObject.Destroy(uObj);
                return;
            }
            SetEffect(effectBehaviour);
        }

        public void Play()
        {
            if(status == EffectStatus.None)
            {
                if(!CachedGameObject.activeSelf)
                {
                    CachedGameObject.SetActive(true);
                }

                status = EffectStatus.Playing;
                if (lifeTime > 0)
                {
                    timer = TimerManager.GetInstance().AddEndTimer(lifeTime, OnLifeTimeEnd);
                }

                effectBehaviour?.Play();
            }
        }

        public void Stop()
        {
            if(status == EffectStatus.Playing)
            {
                status = EffectStatus.Stopping;
                effectBehaviour?.Stop();

                OnLifeTimeEnd(null);
            }
        }

        private void OnLifeTimeEnd(SystemObject data)
        {
            timer = null;
            if (stopDelayTime > 0)
            {
                timer = TimerManager.GetInstance().AddEndTimer(stopDelayTime, OnStopTimeEnd);
            }
            else
            {
                Dead();
            }
        }

        private void OnStopTimeEnd(SystemObject data)
        {
            timer = null;
            Dead();
        }

        private void Dead()
        {
            //effectAssetHandle?.Release();
            effectBehaviour?.Dead();
            status = EffectStatus.Dead;

            effectFinished?.Invoke(this);
            effectFinished = delegate (EffectController e) { };
            effectBehaviour = null;
        }

        public override void DoDespawned()
        {
            if(timer!=null)
            {
                TimerManager.GetInstance().RemoveTimer(timer);
            }
            timer = null;

            isAutoPlayWhenEnable = false;
            lifeTime = 0.0f;
            stopDelayTime = 0.0f;

            status = EffectStatus.None;
            effectBehaviour = null;
            effectFinished = delegate (EffectController e) { };
        }

        private void OnDestroy()
        {
            DoDespawned();
        }
    }
}
