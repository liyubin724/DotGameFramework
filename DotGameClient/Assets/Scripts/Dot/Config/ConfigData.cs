using UnityEngine;

namespace Dot.Config
{
    [CreateAssetMenu(fileName = "config", menuName = "Config Data")]
    public class ConfigData : ScriptableObject
    {
        public BulletConfig bulletConfig;
        public EffectConfig effectConfig;
    }
}
