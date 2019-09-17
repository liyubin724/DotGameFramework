using System.Linq;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public sealed class AssetLoaderHandle
    {
        private long uniqueID;
        private string[] pathOrAddresses;
        private UnityObject[] uObjs;
        private float[] progresses;

        public long UniqueID { get => uniqueID; }
        public string[] PathOrAddresses { get => pathOrAddresses; }
        public string PathOrAddress { get => pathOrAddresses.Length>0?pathOrAddresses[0]:null; }
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

        internal bool isDone = false;
        public bool IsDone { get=>isDone; }

        internal AssetLoaderHandle(long id, string[] paths)
        {
            uniqueID = id;
            pathOrAddresses = paths;

            uObjs = new UnityObject[paths.Length];
            progresses = new float[paths.Length];
        }

        internal void SetObject(int index,UnityObject uObj)
        {
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

        internal void BreakLoader(bool destroyIfLoaded)
        {
            if(destroyIfLoaded)
            {
                for(int i =0;i<uObjs.Length;++i)
                {
                    UnityObject uObj = uObjs[i];
                    if(uObj !=null)
                    {
                        UnityObject.Destroy(uObj);
                        uObjs[i] = null;
                    }
                }
            }
            uniqueID = -1;
            pathOrAddresses = null;
            uObjs = null;
            progresses = null;
        }
    }
}
