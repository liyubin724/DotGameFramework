using System;

namespace Dot.Core.Loader.Config
{
    [Serializable]
    public class AssetGroupConfig
    {
        public string groupName;
        public AssetAddressConfig[] assets = new AssetAddressConfig[0];
    }
}
