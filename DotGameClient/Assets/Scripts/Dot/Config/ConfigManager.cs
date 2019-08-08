using Dot.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Config
{
    public class ConfigManager : Singleton<ConfigManager>
    {
        private EffectConfig effectConfig = new EffectConfig();
        public EffectConfigData GetEffectConfig(int id)
        {
            foreach(var config in effectConfig.configs)
            {
                if(config.id == id)
                {
                    return config;
                }
            }
            return null;
        }

    }
}
