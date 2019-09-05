using System.Linq;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public class AssetLoaderHandle
    {
        private long uniqueID;
        private string[] assetPaths;
        private UnityObject[] uObjs;
        private float[] progresses;
        private bool[] completeCalls;

        public AssetLoaderHandle(long id,string[] paths)
        {
            uniqueID = id;
            assetPaths = paths;
            uObjs = new UnityObject[paths.Length];
            progresses = new float[paths.Length];
            completeCalls = new bool[paths.Length];
            for(int i =0;i<paths.Length;++i)
            {
                progresses[i] = 0.0f;
                completeCalls[i] = false;
            }
        }

        public UnityObject[] GetObjects() => uObjs;
        public UnityObject GetObject(int index = 0)
        {
            if(index>=0&&index<assetPaths.Length)
            {
                return uObjs[index];
            }
            return null;
        }
        internal void SetObject(int index,UnityObject uObj)=> uObjs[index] = uObj;

        public float Progress
        {
            get
            {
                return progresses.Sum((v) => v) / progresses.Length;
            }
        }
        public float[] Progresses() => progresses;
        public float GetProgress(int index = 0)
        {
            if (index >= 0 && index < assetPaths.Length)
            {
                return progresses[index];
            }
            return 0;
        }
        public void SetProgress(int index, float progress) => progresses[index] = progress;
    }
}
