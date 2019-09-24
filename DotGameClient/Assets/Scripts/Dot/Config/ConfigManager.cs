using System;
using System.Collections.Generic;

namespace Dot.Config
{
    public class ConfigManager : Core.Util.Singleton<ConfigManager>
    {
        private static readonly string CONFIG_ADDRESS_NAME = "config_data";

        private ConfigData configData = null;
        private Dictionary<string, string> timeLineConfigDic = new Dictionary<string, string>();
        public void InitConfig(Action finishCallback)
        {
            if(configData!=null)
            {
                finishCallback();
                return;
            }
            //AssetLoader.GetInstance().LoadAssetAsync(CONFIG_ADDRESS_NAME, (address, uObj, userData) => {
            //    configData = uObj as ConfigData;

            //    AssetHandle handle = null;
            //    handle = AssetLoader.GetInstance().LoadAssetsByLabeAsync("timeline_data", (address2, uObj2, userData2) =>
            //    {
            //        timeLineConfigDic.Add(address2, (uObj2 as TextAsset).text);
            //    }, null, (addresses, uObjs, userData3) =>
            //    {
            //        handle.Release();
            //    }, null, null);

            //}, null, null);
        }

        public string GetTimeLineConfig(string path)
        {
            return timeLineConfigDic[path];
        }

        public EffectConfigData GetEffectConfig(int id)
        {
            foreach (var config in configData.effectConfig.configs)
            {
                if (config.id == id)
                {
                    return config;
                }
            }
            return null;
        }

        public EffectConfigData GetEffectConfig(string address)
        {
            foreach (var config in configData.effectConfig.configs)
            {
                if (config.address == address)
                {
                    return config;
                }
            }
            return null;
        }

        public BulletConfigData GetBulletConfig(int id)
        {
            foreach (var config in configData.bulletConfig.configs)
            {
                if (config.id == id)
                {
                    return config;
                }
            }
            return null;
        }

        public SkillConfigData GetSkillConfig(int id)
        {
            foreach(var config in configData.skillConfig.configs)
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
