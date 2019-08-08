using Dot.Core.Asset;
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

            //AssetLoader.GetInstance().InstanceAssetAsync("effect", (address, uObj,userData) => {

            //}, null, null);
            
        }

        private void OnGUI()
        {
            if(GUILayout.Button("Add Pool Effect"))
            {
                EffectController effect = EffectManager.GetInstance().GetEffect("ShipEffect", "effect",false);
                effect.transform.SetParent(transform, false);
                effect.lifeTime = 3;
                effect.effectFinished += delegate (EffectController e)
                {
                    EffectManager.GetInstance().ReleaseEffect(e);
                };
                effect.Play();
            }
            if (GUILayout.Button("Add autoRelease Effect"))
            {
                EffectController effect = EffectManager.GetInstance().GetEffect("ShipEffect2", "effect2");
                effect.transform.SetParent(transform, false);
                effect.lifeTime = 5;
                effect.Play();
            }

            if (GUILayout.Button("Add Effect with no pool"))
            {
                EffectController effect = EffectManager.GetInstance().GetEffect("effect2",false);
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
