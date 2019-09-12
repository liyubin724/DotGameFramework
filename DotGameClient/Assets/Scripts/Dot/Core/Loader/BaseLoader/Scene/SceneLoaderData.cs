using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dot.Core.Loader
{
    public class SceneLoaderData
    {
        public string assetPath;
        public string assetAddress;
        public string sceneName;

        public LoadSceneMode loadMode = LoadSceneMode.Single;
        public bool activateOnLoad = true;

        public AssetLoaderHandle loaderHandle = null;
        public AsyncOperation asyncOperation = null;

        public OnSceneLoadComplete loadComplete;
        public OnSceneLoadProgress loadProgress;
    }
}
