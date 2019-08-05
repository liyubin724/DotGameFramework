using Dot.Core.Asset;
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

        public event OnEffectFinish effectFinished = delegate(EffectController e) { };

        private EffectStatus status = EffectStatus.None;
        private TimerTaskInfo timer = null;
        private AssetHandle effectAssetHandle = null;

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

        private string effectSpawnName = null;
        public void SetEffect(string effectPath, string spawnName=null)
        {
            effectSpawnName = spawnName;
            effectAssetHandle = AssetLoader.GetInstance().InstanceAssetAsync(effectPath, OnEffectLoadComplete, null,null);
        }

        private void OnEffectLoadComplete(string effectPath,UnityObject uObj,SystemObject userData)
        {
            effectAssetHandle = null;
            GameObject effectGO = (GameObject)uObj;
            if(effectGO!=null)
            {
                EffectBehaviour effectBehaviour = effectGO.GetComponent<EffectBehaviour>();
                if(effectBehaviour)
                {
                    effectBehaviour.AssetPath = effectPath;
                    effectBehaviour.SpawnName = effectSpawnName;

                    SetEffect(effectBehaviour);
                }else
                {
                    GameObject.Destroy(effectBehaviour);
                }
            }
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
                    timer = GameApplication.GTimer.AddTimerTask(lifeTime, lifeTime, null, null, OnLifeTimeEnd, null);
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
            StopTimer();
            if (stopDelayTime > 0)
            {
                timer = GameApplication.GTimer.AddTimerTask(stopDelayTime, stopDelayTime, null, null, OnStopTimeEnd, null);
            }
            else
            {
                Dead();
            }
        }

        private void OnStopTimeEnd(SystemObject data)
        {
            StopTimer();
            Dead();
        }

        private void Dead()
        {
            effectAssetHandle?.Release();
            effectBehaviour?.Dead();
            status = EffectStatus.Dead;

            effectFinished?.Invoke(this);
            effectFinished = delegate (EffectController e) { };
            effectBehaviour = null;
        }

        private void StopTimer()
        {
            if (timer != null)
            {
                GameApplication.GTimer.RemoveTimerTask(timer);
            }
            timer = null;
        }

        public override void DoDespawned()
        {
            StopTimer();

            isAutoPlayWhenEnable = false;
            lifeTime = 0.0f;
            stopDelayTime = 0.0f;

            status = EffectStatus.None;
            effectBehaviour = null;
            effectFinished = delegate (EffectController e) { };
            effectSpawnName = null;
        }

        private void OnDestroy()
        {
            DoDespawned();
        }
    }
}
