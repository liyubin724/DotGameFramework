using System.Linq;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public sealed class AssetLoaderHandle
    {
        private long uniqueID;
        private string[] assetPaths;
        private UnityObject[] uObjs;
        private float[] progresses;
        private bool[] states;

        public long UniqueID { get => uniqueID; }
        public string[] AssetPaths { get => assetPaths; }
        public string AssetPath { get => assetPaths.Length>0?assetPaths[0]:null; }
        public UnityObject[] AssetObjects { get => uObjs; }
        public UnityObject AssetObject { get => uObjs.Length > 0 ? uObjs[0] : null; }
        public float[] AssetProgresses { get => progresses; }
        public float AssetProgress { get => progresses.Length > 0 ? progresses[0] : 0.0f; }
        public float TotalProgress
        {
            get
            {
                if (progresses == null) return 0.0f;

                return progresses.Sum((v) => v) / progresses.Length;
            }
        }

        internal AssetLoaderHandle(long id, string[] paths)
        {
            uniqueID = id;
            assetPaths = paths;

            uObjs = new UnityObject[paths.Length];
            progresses = new float[paths.Length];
            states = new bool[paths.Length];

            for (int i = 0; i < paths.Length; ++i)
            {
                progresses[i] = 0.0f;
                states[i] = false;
            }
        }

        internal bool GetAssetState(int index)
        {
            return states[index];
        }

        internal void SetObject(int index,UnityObject uObj)
        {
            states[index] = true;
            uObjs[index] = uObj;
        }

        internal UnityObject GetObject(int index)
        {
            return uObjs[index];
        }

        internal void SetProgress(int index,float progress)
        {
            progresses[index] = progress;
        }

        internal float GetProgress(int index)
        {
            return progresses[index];
        }
    }
}
