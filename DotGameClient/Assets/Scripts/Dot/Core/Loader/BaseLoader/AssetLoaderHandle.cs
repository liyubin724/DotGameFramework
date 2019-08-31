﻿using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public class AssetLoaderHandle
    {
        private long uniqueID;
        private string[] assetPaths;
        private UnityObject[] uObjs;
        private float[] progresses;

        public AssetLoaderHandle(long id,string[] paths)
        {
            uniqueID = id;
            assetPaths = paths;
            uObjs = new UnityObject[paths.Length];
            progresses = new float[paths.Length];
            for(int i =0;i<paths.Length;++i)
            {
                progresses[i] = 0.0f;
            }
        }

        public string AssetPath
        {
            get
            {
                if(assetPaths!=null && assetPaths.Length>0)
                {
                    return assetPaths[0];
                }
                return null;
            }
        }

        public string[] AssetPaths { get => assetPaths; }

        public UnityObject GetObject(int index = 0)
        {
            if(index>=0&&index<assetPaths.Length)
            {
                return uObjs[index];
            }
            return null;
        }

        public float GetProgress(int index = 0)
        {
            if (index >= 0 && index < assetPaths.Length)
            {
                return progresses[index];
            }
            return 0;
        }
    }
}
