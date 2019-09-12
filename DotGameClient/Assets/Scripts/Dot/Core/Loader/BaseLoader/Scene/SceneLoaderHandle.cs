using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dot.Core.Loader
{
    public sealed class SceneLoaderHandle
    {
        private string assetAddress;
        private string assetPath;
        private string sceneName;
        private float progress;
        private Scene scene;

        public void SetSceneActive(bool isActive)
        {
            if(scene.isLoaded)
            {
                GameObject[] gObjs = scene.GetRootGameObjects();
                foreach(var go in gObjs)
                {
                    go.SetActive(isActive);
                }
            }else
            {
                Debug.LogError("");
            }
        }
    }
}

