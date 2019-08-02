using Dot.Core.Pool;
using UnityEngine;

namespace Dot.Core.Effect
{

    public class EffectBehaviour : GameObjectPoolItem
    {
        private Animator[] animators = new Animator[0];
        private ParticleSystem[] particleSystems = new ParticleSystem[0];

        public void Play()
        {
            if (!CachedGameObject.activeSelf)
            {
                CachedGameObject.SetActive(true);
            }
        }

        public void Stop()
        {
        }

        public void Dead()
        {

        }

        public override void DoSpawned()
        {
            
        }

        public override void DoDespawned()
        {

        }
    }
}
