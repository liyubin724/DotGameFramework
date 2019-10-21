using UnityEngine;
using UnityEngine.SceneManagement;
using SystemObject = System.Object;

namespace Dot.Core.Loader
{
    public class SceneLoadData
    {
        private static float DEPEND_ASSET_PROGRESS_RATE = 0.9f;

        public string pathOrAddress;
        public string assetPath;
        public LoadSceneMode loadMode = LoadSceneMode.Single;
        public bool activateOnLoad = true;
        public OnSceneLoadComplete completeCallback;
        public OnSceneLoadProgress progressCallback;
        public SystemObject userData;

        public SceneLoaderHandle handle;
        public AssetLoaderHandle loaderHandle = null;
        public AsyncOperation asyncOperation = null;

        public bool IsLoading() => loaderHandle != null || asyncOperation != null;

        public bool IsAssetLoaderDone()
        {
            if (loaderHandle == null) return false;
            return loaderHandle.state == AssetLoaderState.Complete;
        }

        public bool IsOperationDone()
        {
            if (asyncOperation == null) return false;
            return asyncOperation.isDone;
        }

        public float Progress()
        {
            float progress = 0.0f;
            if (loaderHandle != null)
            {
                progress = loaderHandle.TotalProgress * DEPEND_ASSET_PROGRESS_RATE;
                if (asyncOperation != null)
                {
                    progress += asyncOperation.progress * (1 - DEPEND_ASSET_PROGRESS_RATE);
                }
            }
            else
            {
                if (asyncOperation != null)
                {
                    progress = asyncOperation.progress;
                }
            }
            return progress;
        }
    }

    public class SceneUnloadData
    {
        public string pathOrAddress;
        public OnSceneUnloadComplete completeCallback;
        public OnSceneUnloadProgress progressCallback;
        public SystemObject userData;
        
        public AsyncOperation asyncOperation = null;

        public bool IsDone()
        {
            if (asyncOperation != null)
            {
                return asyncOperation.isDone;
            }
            return false;
        }

        public bool IsUnloading()
        {
            return asyncOperation != null;
        }

        public float Progress()
        {
            if (asyncOperation != null)
            {
                return asyncOperation.progress;
            }
            return 0.0f;
        }
    }

}
