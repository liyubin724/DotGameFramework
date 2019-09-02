using Dot.Core.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Core.Loader
{
    public class ResourceLoaderData : AssetLoaderData, IObjectPoolItem
    {
        internal ResourceAsyncOperation[] asyncOperations = null;

        internal void Init()
        {
            asyncOperations = new ResourceAsyncOperation[assetPaths.Length];
        }

        public void OnNew()
        {
            
        }

        public void OnRelease()
        {
            Reset();
            asyncOperations = null;
        }
    }
}
