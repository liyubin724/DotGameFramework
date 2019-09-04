using Dot.Core.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Core.Loader
{
    public class AssetBundleLoaderData : AssetLoaderData, IObjectPoolItem
    {
        
        public void OnNew()
        {
        }

        public void OnRelease()
        {
            Reset();
        }
    }
}
