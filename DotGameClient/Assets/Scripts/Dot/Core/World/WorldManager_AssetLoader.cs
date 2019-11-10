using Dot.Core.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Core.World
{
    public partial class WorldManager
    {
        private AssetLoaderBridge loaderBridge = null;
        private void DoInitAssetLoader()
        {
            loaderBridge = new AssetLoaderBridge(AssetLoaderPriority.Default);
        }
    }
}
