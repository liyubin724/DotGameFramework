using Dot.Core.Pool;
using Dot.Core.Timer;
using UnityEngine;
using SystemObject = System.Object;

namespace Dot.Core.Effect
{

    public delegate void OnEffectDead(EffectBehaviour effect);

    public class EffectBehaviour : GameObjectPoolItem
    {
        public bool isAutoPlayWhenEnable = true;
        public float lifeTime = 0.0f;

        public float stopDelayTime = 0.0f;

        private Animator[] animators = new Animator[0];
        private ParticleSystem[] particleSystems = new ParticleSystem[0];

        public event OnEffectDead effectDeadCallback = delegate (EffectBehaviour e) { };

        private TimerTaskInfo lifeTimer = null;
        public void Start()
        {
            
        }

        public void OnEnable()
        {
            if (isAutoPlayWhenEnable)
                Play();
        }

        public void Play()
        {
            if(lifeTime>0)
            {
                lifeTimer = GameApplication.GTimer.AddTimerTask(lifeTime, lifeTime, null, null, OnLifeTimeEnd, null);
            }
        }

        public void Stop()
        {
            OnLifeTimeEnd(null);
        }

        public override void DoSpawned()
        {
            
        }

        public override void DoDespawned()
        {
            StopTimer();
            effectDeadCallback = delegate (EffectBehaviour e) { };
        }

        private void OnLifeTimeEnd(SystemObject data)
        {
            StopTimer();
            if(stopDelayTime>0)
            {
                lifeTimer = GameApplication.GTimer.AddTimerTask(lifeTime, lifeTime, null, null, OnLifeTimeEnd, null);
            }else
            {
                Dead();
            }
        }

        private void OnStopDelayTimeEnd(SystemObject data)
        {
            StopTimer();
            Dead();
        }

        private void Dead()=> effectDeadCallback(this);

        private void StopTimer()
        {
            if (lifeTimer != null)
            {
                GameApplication.GTimer.RemoveTimerTask(lifeTimer);
            }
            lifeTimer = null;
        }

        private void OnDestroy()
        {
            StopTimer();
        }

    }
}
