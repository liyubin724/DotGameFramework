using UnityEngine;

namespace Dot.Config
{
   // [CreateAssetMenu(fileName = "config", menuName = "Config Data")]
    public class ConfigData : ScriptableObject
    {
        public BulletConfig bulletConfig;
        public EffectConfig effectConfig;
        public SkillConfig skillConfig;

        public EffectConfigData GetEffect(string address)
        {
            if (effectConfig == null) return null;

            foreach(var effect in effectConfig.configs)
            {
                if(effect.address == address)
                {
                    return effect;
                }
            }
            return null;
        }

    }
}
