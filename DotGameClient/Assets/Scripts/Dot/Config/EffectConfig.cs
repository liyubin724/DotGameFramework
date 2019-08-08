using UnityEngine;
using System;
using System.Collections.Generic;

namespace Dot.Config
{
    [Serializable]
    public class EffectConfigData
    {
        public int id;
        public string address;
        public bool isAutoPlay = true;
        public float lifeTime;
        public float stopDelayTime;
    }

    public class EffectConfig : ScriptableObject
    {
        public List<EffectConfigData> configs = new List<EffectConfigData>();
    }
}
