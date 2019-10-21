using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using SystemObject = System.Object;

namespace Dot.Core.Loader
{
    public class SceneAssetLoader
    {
        private AssetLoaderMode loaderMode;
        private AAssetLoader assetLoader;

        private List<SceneLoaderHandle> loadedSceneHandles = new List<SceneLoaderHandle>();

        private List<SceneLoadData> loadingSceneDatas = new List<SceneLoadData>();
        private List<SceneUnloadData> unloadingSceneDatas = new List<SceneUnloadData>();

        public SceneAssetLoader(AssetLoaderMode loaderMode, AAssetLoader assetLoader)
        {
            this.loaderMode = loaderMode;
            this.assetLoader = assetLoader;
        }

        public SceneLoaderHandle LoadSceneAsync(string pathOrAddress,
            OnSceneLoadComplete completeCallback,
            OnSceneLoadProgress progressCallback,
            LoadSceneMode loadMode = LoadSceneMode.Single,
            bool activateOnLoad = true,
            SystemObject userData = null)
        {
            bool isSceneLoaded = loadedSceneHandles.Any((sHandle) =>
            {
                return sHandle.pathOrAddress == pathOrAddress;
            });
            if (isSceneLoaded)
            {
                Debug.LogError($"SceneAssetLoader::LoadSceneAsync->Scene has been loaded.pathOrAddress={pathOrAddress}");
                return null;
            }
            bool isSceneLoading = loadingSceneDatas.Any((loadData) =>
            {
                return loadData.pathOrAddress == pathOrAddress;
            });
            if (isSceneLoading)
            {
                Debug.LogError($"SceneAssetLoader::LoadSceneAsync->Scene is in loading.pathOrAddress={pathOrAddress}");
                return null;
            }

            string assetPath = assetLoader.GetAssetPath(pathOrAddress);
            string sceneName = Path.GetFileNameWithoutExtension(assetPath);

            SceneLoadData loaderData = new SceneLoadData();
            loaderData.pathOrAddress = pathOrAddress;
            loaderData.assetPath = assetPath;
            loaderData.completeCallback = completeCallback;
            loaderData.progressCallback = progressCallback;
            loaderData.loadMode = loadMode;
            loaderData.activateOnLoad = activateOnLoad;
            loaderData.userData = userData;
            
            SceneLoaderHandle handle = new SceneLoaderHandle();
            handle.pathOrAddress = pathOrAddress;
            handle.assetPath = assetPath;
            handle.sceneName = sceneName;
            loaderData.handle = handle;

            loadingSceneDatas.Add(loaderData);

            return handle;
        }

        public void UnloadSceneAsync(string pathOrAddress,
            OnSceneUnloadComplete completeCallback,
            OnSceneUnloadProgress progressCallback,
            SystemObject userData = null)
        {
            bool isSceneLoaded = loadedSceneHandles.Any((sHandle) =>
            {
                return sHandle.pathOrAddress == pathOrAddress;
            });
            if (!isSceneLoaded)
            {
                Debug.LogError($"SceneAssetLoader::UnloadSceneAsync->Scene not found.pathOrAddress={pathOrAddress}");
                return;
            }

            SceneUnloadData unloadData = new SceneUnloadData();
            unloadData.pathOrAddress = pathOrAddress;
            unloadData.completeCallback = completeCallback;
            unloadData.progressCallback = progressCallback;
            unloadData.userData = userData;

            unloadingSceneDatas.Add(unloadData);
        }

        internal void DoUpdate(float deltaTime)
        {
            if (loadingSceneDatas.Count > 0)
            {
                SceneLoadData loadData = loadingSceneDatas[0];
                if (IsSceneLoadComplete(loadData))
                {
                    loadingSceneDatas.RemoveAt(0);
                    SceneLoadComplete(loadData);
                } else if (!loadData.IsLoading())
                {
                    SceneLoadStart(loadData);
                } else
                {
                    SceneLoadProgress(loadData);
                }
            }
            if (unloadingSceneDatas.Count > 0)
            {
                SceneUnloadData unloadData = unloadingSceneDatas[0];
                if (unloadData.IsDone())
                {
                    unloadingSceneDatas.RemoveAt(0);
                    SceneUnloadComplete(unloadData);
                } else if (unloadData.IsUnloading())
                {
                    unloadData.progressCallback?.Invoke(unloadData.pathOrAddress, unloadData.Progress(), unloadData.userData);
                } else
                {
                    SceneUnloadStart(unloadData);
                }
            }
        }

        private bool IsSceneLoadComplete(SceneLoadData loadData)
        {
            if (loaderMode == AssetLoaderMode.AssetDatabase)
            {
                return loadData.IsOperationDone();
            }
            else if (loaderMode == AssetLoaderMode.AssetBundle)
            {
                return loadData.IsOperationDone() && loadData.IsAssetLoaderDone();
            }
            else if (loaderMode == AssetLoaderMode.Resources)
            {

            }
            Debug.LogError($"SceneAssetLoader::IsSceneLoadComplete->Unvalid AssetLoaderMode.loaderMode = {loaderMode}");
            return false;
        }

        private void SceneLoadStart(SceneLoadData loadData)
        {
            if (loaderMode == AssetLoaderMode.AssetDatabase)
            {
#if UNITY_EDITOR
                loadData.asyncOperation = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(loadData.assetPath, new LoadSceneParameters(loadData.loadMode));
#endif
            }
            else if (loaderMode == AssetLoaderMode.AssetBundle)
            {
                loadData.loaderHandle = assetLoader.LoadOrInstanceBatchAssetAsync(new string[] { loadData.pathOrAddress }, false, AssetLoaderPriority.High, null, null, null, null, null);
            }
            else if (loaderMode == AssetLoaderMode.Resources)
            {

            }
        }

        private void SceneLoadProgress(SceneLoadData loadData)
        {
            if (loaderMode == AssetLoaderMode.AssetBundle && loadData.IsAssetLoaderDone() && loadData.asyncOperation == null)
            {
                string sceneName = Path.GetFileNameWithoutExtension(loadData.assetPath);
                loadData.asyncOperation = SceneManager.LoadSceneAsync(sceneName, loadData.loadMode);
            }

            float progress = loadData.Progress();
            float oldProgress = loadData.handle.progress;
            if (progress != oldProgress)
            {
                loadData.handle.progress = progress;

                loadData.progressCallback?.Invoke(loadData.pathOrAddress, progress, loadData.userData);
            }
        }

        private void SceneLoadComplete(SceneLoadData loadData)
        {
            if (loadData.loadMode == LoadSceneMode.Single)
            {
                foreach (var handle in loadedSceneHandles)
                {
                    assetLoader.UnloadAsset(handle.pathOrAddress);
                }
                loadedSceneHandles.Clear();
            }

            SceneLoaderHandle loaderHandle = loadData.handle;
            Scene scene = SceneManager.GetSceneByName(loaderHandle.SceneName);
            loaderHandle.SetScene(scene);

            loadedSceneHandles.Add(loaderHandle);
            loadData.completeCallback?.Invoke(loadData.pathOrAddress, scene, loadData.userData);
        }

        private void SceneUnloadComplete(SceneUnloadData unloadData)
        {
            SceneLoaderHandle loaderHandle = null;
            foreach(var handle in loadedSceneHandles)
            {
                if(handle.pathOrAddress == unloadData.pathOrAddress)
                {
                    loaderHandle = handle;
                    break;
                }
            }
            loadedSceneHandles.Remove(loaderHandle);

            assetLoader.UnloadAsset(loaderHandle.pathOrAddress);

            unloadData.completeCallback?.Invoke(unloadData.pathOrAddress, unloadData.userData);
        }

        private void SceneUnloadStart(SceneUnloadData unloadData)
        {
            SceneLoaderHandle loaderHandle = null;
            foreach (var handle in loadedSceneHandles)
            {
                if (handle.pathOrAddress == unloadData.pathOrAddress)
                {
                    loaderHandle = handle;
                    break;
                }
            }
            unloadData.asyncOperation = SceneManager.UnloadSceneAsync(loaderHandle.SceneName);
        }
    }
}
