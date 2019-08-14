using UnityEngine;
using System;
using System.Collections.Generic;

namespace Dot.Config
{
    [Serializable]
    public class EffectConfigData
    {
        public string address;
        public bool isAutoPlay = true;
        public float lifeTime;
        public float stopDelayTime;


        public int id;
    }

    [Serializable]
    public class EffectConfig 
    {
        public List<EffectConfigData> configs = new List<EffectConfigData>();
    }
}
