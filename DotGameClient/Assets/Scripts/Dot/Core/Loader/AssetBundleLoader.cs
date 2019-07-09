using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dot.Core.Loader
{
    public class AssetBundleAsyncOperationData : AsyncOperationData
    {
        public AssetBundleAsyncOperationData(string path, AssetLoaderPriorityType priorityType) : base(path, priorityType)
        {
        }

        public override void CreateOperation()
        {
            operation = AssetBundle.LoadFromFileAsync(path);
        }

        public override UnityEngine.Object GetAsset()
        {
            return (operation as AssetBundleCreateRequest).assetBundle;
        }
    }

    public class AssetBundleLoader : AAssetLoader
    {
        public override void CancelAssetLoader(int index)
        {
            throw new NotImplementedException();
        }

        public override UnityEngine.Object LoadAsset(string assetFullPath)
        {
            throw new NotImplementedException();
        }

        public override int LoadAssetAsync(AssetAsyncData data)
        {
            throw new NotImplementedException();
        }

        public override UnityEngine.Object[] LoadBatchAsset(string[] assetFullPaths)
        {
            throw new NotImplementedException();
        }

        public override int LoadBatchAssetAsync(BatchAssetAsyncData data)
        {
            throw new NotImplementedException();
        }

        public override void UnloadUnusedAsset()
        {
            throw new NotImplementedException();
        }

        protected override string GetAssetPath(string assetFullPath)
        {
            throw new NotImplementedException();
        }
    }
}
