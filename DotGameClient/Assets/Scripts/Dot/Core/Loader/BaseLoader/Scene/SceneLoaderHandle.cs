using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dot.Core.Loader
{
    public sealed class SceneLoaderHandle
    {
        internal string pathOrAddress;
        internal string assetPath;
        internal string sceneName;
        internal float progress =0.0f;
        internal Scene scene;

        public string AssetPath { get => assetPath;  }
        public string PathOrAddress { get => pathOrAddress; }
        public string SceneName { get => sceneName;  }
        public float Progress { get => progress; }

        private bool isActive = true;
        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                if(isActive!=value)
                {
                    isActive = value;
                    if(scene.isLoaded)
                    {
                        SetSceneActive(isActive);
                    }
                }
            }
        }

        internal void SetScene(Scene scene)
        {
            this.scene = scene;
            if(!isActive)
            {
                SetSceneActive(isActive);
            }
        }

        private void SetSceneActive(bool isActive)
        {
            GameObject[] gObjs = scene.GetRootGameObjects();
            foreach (var go in gObjs)
            {
                go.SetActive(isActive);
            }
        }
    }
}

