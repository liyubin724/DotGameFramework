using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemObject = System.Object;

namespace Dot.Core.Resource
{
    public enum ResourceMode
    {
        AssetDatabase,
        Resources,
        AssetBundle,
        PackedAssetBundle,
    }

    public class AResourceLoader
    {
        public void LoadAssetAsync(string assetPath,Action finishCallback,Action progressCallback,SystemObject userData)
        {

        }

        public void LoadAssetsAsync(string[] assetPathArr,Action finishCallback,Action progressCallback,SystemObject userData)
        {

        }

        public void InstanceAssetAsync(string assetPath,Action finishCallback,Action progressCallback,SystemObject userData)
        {

        }

        public void InstanceAssetsAsync(string[] assetPathArr,Action finishCallback,Action progressCallback,SystemObject userData)
        {

        }

        public void UnloadUnusedAssets()
        {

        }

    }
}
