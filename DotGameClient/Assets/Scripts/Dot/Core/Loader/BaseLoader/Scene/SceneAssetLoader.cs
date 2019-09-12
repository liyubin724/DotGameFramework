using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using SystemObject = System.Object;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public class SceneAssetLoader
    {
        private AssetLoaderMode loaderMode;
        private AAssetLoader assetLoader;

        private Dictionary<string, SceneLoaderHandle> loaderHandleDic = new Dictionary<string, SceneLoaderHandle>();
        private List<SceneLoaderData> loaderDataList = new List<SceneLoaderData>();

        public SceneAssetLoader(AssetLoaderMode loaderMode,AAssetLoader assetLoader)
        {
            this.loaderMode = loaderMode;
            this.assetLoader = assetLoader;
        }

        public SceneLoaderHandle LoadSceneAsync(string pathOrAddress, 
            OnSceneLoadComplete completeCallback,
            OnSceneLoadProgress progressCallback,
            LoadSceneMode loadMode = LoadSceneMode.Single, 
            bool activateOnLoad = true,
            AssetLoaderPriority priority = AssetLoaderPriority.High)
        {
            if(loaderHandleDic.ContainsKey(pathOrAddress))
            {
                Debug.LogError("SceneAssetLoader::LoadSceneAsync->Scene has been loaded");
                return null;
            }


            return null;
        }

        public void UnloadSceneAsync(string pathOrAddress,
            OnSceneUnloadComplete completeCallback,
            OnSceneUnloadProgress progressCallback)
        {
            
        }

        private void OnAssetBundleSceneLoadComplete(string pathOrAddress,UnityObject uObj,SystemObject userData)
        {

        }

        private void OnAssetBundleSceneLoadProgress(string pathOrAddress,float progress,SystemObject userData)
        {

        }

        internal void DoUpdate(float deltaTime)
        {

        }
    }
}
