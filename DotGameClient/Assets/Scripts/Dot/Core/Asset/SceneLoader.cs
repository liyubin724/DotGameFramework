using System.Collections.Generic;
//using UnityEngine.AddressableAssets;
//using UnityEngine.ResourceManagement.AsyncOperations;
//using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Dot.Core.Asset
{
    public delegate void OnSceneLoadProgressCallback(string address, float progress);
    public delegate void OnSceneLoadFinishCallback(string address, SceneInstanceData scene);

    public delegate void OnSceneUnloadProgressCallback(string address, float progress);
    public delegate void OnSceneUnloadFinishCallback(string address);

    public class SceneLoadData
    {
        public string address;
        public OnSceneLoadProgressCallback progress;
        public OnSceneLoadFinishCallback finish;
    }

    public class SceneUnloadData
    {
        public string address;
        public OnSceneUnloadProgressCallback progress;
        public OnSceneUnloadFinishCallback finish;
    }

    public class SceneInstanceData
    {
        public string address;
        //public SceneInstance scene;
    }

    public class SceneLoader : Util.Singleton<SceneLoader>
    {
        //private Dictionary<AsyncOperationHandle<SceneInstance>, SceneLoadData> sceneLoadDic = new Dictionary<AsyncOperationHandle<SceneInstance>, SceneLoadData>();
        //private Dictionary<AsyncOperationHandle<SceneInstance>, SceneUnloadData> sceneUnloadDic = new Dictionary<AsyncOperationHandle<SceneInstance>, SceneUnloadData>();
        
        public void UnloadScene(SceneInstanceData scene, OnSceneUnloadProgressCallback progress = null, OnSceneUnloadFinishCallback finish = null)
        {
            SceneUnloadData unloadData = new SceneUnloadData()
            {
                address = scene.address,
                progress = progress,
                finish = finish,
            };
            //AsyncOperationHandle<SceneInstance> handle = Addressables.UnloadSceneAsync(scene.scene, false);
            //sceneUnloadDic.Add(handle, unloadData);
        }

        public void LoadSceneAsync(string address, OnSceneLoadProgressCallback progress = null, OnSceneLoadFinishCallback finish = null)
        {
            LoadSceneAsync(address, LoadSceneMode.Single, true, progress, finish);
        }

        public void LoadSceneAsync(string address, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true,
                                                    OnSceneLoadProgressCallback progress = null, OnSceneLoadFinishCallback finish = null)
        {
            SceneLoadData loadData = new SceneLoadData()
            {
                address = address,
                progress = progress,
                finish = finish,
            };

            //AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(address, loadMode, activateOnLoad);

            //sceneLoadDic.Add(handle, loadData);
        }

        //private List<AsyncOperationHandle<SceneInstance>> removedKeyList = new List<AsyncOperationHandle<SceneInstance>>();
        public void DoUpdate()
        {
            //if(sceneUnloadDic.Count>0)
            //{
            //    foreach(var kvp in sceneUnloadDic)
            //    {
            //        AsyncOperationHandle<SceneInstance> handle = kvp.Key;
            //        SceneUnloadData unloadData = kvp.Value;

            //        if(handle.Status == AsyncOperationStatus.Succeeded || handle.Status == AsyncOperationStatus.Failed)
            //        {
            //            unloadData.progress?.Invoke(unloadData.address, 1.0f);
            //            unloadData.finish?.Invoke(unloadData.address);
            //            removedKeyList.Add(handle);
            //        }else
            //        {
            //            unloadData.progress?.Invoke(unloadData.address, handle.PercentComplete);
            //        }
            //    }

            //    if(removedKeyList.Count>0)
            //    {
            //        removedKeyList.ForEach((handle) =>
            //        {
            //            Addressables.Release(handle);
            //            sceneUnloadDic.Remove(handle);
            //        });
            //        removedKeyList.Clear();
            //    }
            //}

            //if(sceneLoadDic.Count>0)
            //{
            //    foreach (var kvp in sceneLoadDic)
            //    {
            //        AsyncOperationHandle<SceneInstance> handle = kvp.Key;
            //        SceneLoadData loadData = kvp.Value;

            //        if (handle.Status == AsyncOperationStatus.Succeeded || handle.Status == AsyncOperationStatus.Failed)
            //        {
            //            SceneInstanceData instanceData = new SceneInstanceData()
            //            {
            //                address = loadData.address,
            //                scene = handle.Result,
            //            };
            //            loadData.progress?.Invoke(loadData.address, 1.0f);
            //            loadData.finish?.Invoke(loadData.address,instanceData);
            //            removedKeyList.Add(handle);
            //        }
            //        else
            //        {
            //            loadData.progress?.Invoke(loadData.address, handle.PercentComplete);
            //        }
            //    }

            //    if (removedKeyList.Count > 0)
            //    {
            //        removedKeyList.ForEach((handle) =>
            //        {
            //            sceneLoadDic.Remove(handle);
            //        });
            //        removedKeyList.Clear();
            //    }
            //}
        }

    }
}
