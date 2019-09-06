using System;

namespace Dot.Core.Loader.Config
{
    [Serializable]
    public class AssetAddressData
    {
        public string assetAddress;
        public string assetPath;
        public string bundlePath;
        public string[] labels = new string[0];
    }
}
