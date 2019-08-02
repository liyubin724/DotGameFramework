using Dot.Core.Effect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dot.Tests
{
    public class TestEffect : MonoBehaviour
    {
        private void Start()
        {
            EffectManager.GetInstance().initFinishCallback = () =>
            {
                EffectManager.GetInstance().PreloadEffect("ShipEffect", "effect", 10, (spawnName, address) =>
                {
                    Debug.Log("Preload Finished");
                });
            };

            
        }

        private void OnGUI()
        {
            if(GUILayout.Button("Add Pool Effect"))
            {
                EffectController effect = EffectManager.GetInstance().GetEffect("ShipEffect", "effect");
                effect.transform.SetParent(transform, false);
                effect.lifeTime = 3;
                effect.effectFinished += delegate (EffectController e)
                {
                    EffectManager.GetInstance().ReleaseEffect(effect);
                };
                effect.Play();
            }
            if (GUILayout.Button("Add autoPool Effect"))
            {
                EffectController effect = EffectManager.GetInstance().GetEffect("ShipEffect2", "effect2");
                effect.transform.SetParent(transform, false);
                effect.lifeTime = 3;
                effect.effectFinished += delegate (EffectController e)
                {
                    EffectManager.GetInstance().ReleaseEffect(effect);
                };
                effect.Play();
            }

            if (GUILayout.Button("Add Single Effect"))
            {
                EffectController effect = EffectManager.GetInstance().GetEffect("effect2");
                effect.transform.SetParent(transform, false);
                effect.lifeTime = 3;
                effect.effectFinished += delegate (EffectController e)
                {
                    EffectManager.GetInstance().ReleaseEffect(effect);
                };
                effect.Play();
            }

            if(GUILayout.Button("Clear Pool"))
            {
                EffectManager.GetInstance().CleanSpawnPool("ShipEffect2");
            }
        }
    }
}
