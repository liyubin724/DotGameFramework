using Dot.Core.Pool;
using UnityEngine;
#if !NOT_ETERNITY
using Game.VFXController;
#endif

namespace Dot.Core.Effect
{
    public class EffectBehaviour : GameObjectPoolItem
    {
#if !NOT_ETERNITY
        [SerializeField]
        private Animator[] animators = new Animator[0];
        [SerializeField]
        private ParticleSystem[] particleSystems = new ParticleSystem[0];
        [SerializeField]
        private VFXLineRendererTrail[] lineTrails = new VFXLineRendererTrail[0];
        [SerializeField]
        private VFXBeam[] beams = new VFXBeam[0];
        [SerializeField]
        private TrailRenderer[] trailRenderers = new TrailRenderer[0];
        [SerializeField]
        private MeshRenderer[] meshRenderers = new MeshRenderer[0];
        [SerializeField]
        private VFXCameraShakeComponent[] cameraShakes = new VFXCameraShakeComponent[0];
        [SerializeField]
        private VFXPostEffectComponent[] postEffects = new VFXPostEffectComponent[0];
        [SerializeField]
        private VFXReplaceMeshWithSpacecraft[] replaceMeshs = new VFXReplaceMeshWithSpacecraft[0];

        public bool isMainPlayer = false;
#endif
        public void Play()
        {
            if (!CachedGameObject.activeSelf)
            {
                CachedGameObject.SetActive(true);
            }
#if !NOT_ETERNITY
            foreach (var animator in animators)
            {
                animator.SetTrigger("start");
            }
            foreach(var particleSystem in particleSystems)
            {
                particleSystem.Clear();
                particleSystem.Play();
            }
            foreach(var beam in beams)
            {
                beam.StartFX();
            }
            foreach(var meshRenderer in meshRenderers)
            {
                meshRenderer.enabled = true;
            }
            foreach(var trail in trailRenderers)
            {
                trail.Clear();
                trail.emitting = true;
            }

            for (int iShake = 0; iShake < cameraShakes.Length; iShake++)
            {
                if (cameraShakes[iShake] != null && ((cameraShakes[iShake].OnlyForMainPlayer && isMainPlayer) || !cameraShakes[iShake].OnlyForMainPlayer))
                {
                    cameraShakes[iShake].Apply();
                }
            }

            for (int iPost = 0; iPost < postEffects.Length; iPost++)
            {
                VFXPostEffectComponent postEffect = postEffects[iPost];
                if (postEffect != null && ((postEffect.OnlyForMainPlayer && isMainPlayer) || !postEffect.OnlyForMainPlayer))
                {
                    Camera curCam = null;
                    MainCameraComponent mainCamera = CameraManager.GetInstance().GetMainCamereComponent();
                    if (mainCamera != null)
                    {
                        curCam = mainCamera.GetCamera();
                    }
                    else
                    {
                        curCam = Camera.main;
                    }

                    postEffect.SetRadialBlueCenter(postEffect.GetInstanceID()
                                                , Camera.main.WorldToViewportPoint(transform.position));
                    postEffect.ApplyPostEffect();
                }
            }

            for (int iEffect = 0; iEffect < replaceMeshs.Length; iEffect++)
            {
                replaceMeshs[iEffect].Apply();
            }
#endif
        }

        public void Stop()
        {
#if !NOT_ETERNITY
            foreach (var animator in animators)
            {
                animator.SetTrigger("stop");
            }
            foreach (var particleSystem in particleSystems)
            {
                particleSystem.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            }
            foreach(var line in lineTrails)
            {
                line.Kill();
            }
            foreach (var beam in beams)
            {
                beam.StopFX();
            }
            foreach (var meshRenderer in meshRenderers)
            {
                meshRenderer.enabled = false;
            }
            foreach (var trail in trailRenderers)
            {
                trail.emitting = false;
            }
#endif
        }

        public void Dead()
        {
            CachedGameObject.SetActive(false);
        }

        public override void DoSpawned()
        {
            
        }

        public override void DoDespawned()
        {

        }
    }
}
