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
            EffectManager.GetInstance().PreloadEffect("ShipEffect", "effect", 10,(spawnName, address) =>
            {
                Debug.Log("Preload Finished");
            });
        }

        private void OnGUI()
        {
            if(GUILayout.Button("Add Effect"))
            {
                EffectBehaviour effect = EffectManager.GetInstance().GetEffect("effect");
                effect.transform.SetParent(transform, false);
            }
        }
    }
}
