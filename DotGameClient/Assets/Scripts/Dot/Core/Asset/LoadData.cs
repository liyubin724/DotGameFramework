using Dot.Core.Generic;
using SystemObject = System.Object;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Asset
{
    public class LoadData : IORMData<long>
    {
        public long uniqueID = -1;
        public OnAssetLoadFinishCallback singleFinish = null;
        public OnAssetLoadProgressCallback singleProgress = null;
        public OnAssetsLoadFinishCallback allFinish = null;
        public OnAssetsLoadProgressCallback allProgress = null;
        public bool isInstance = false;
        public SystemObject userData;

        private float[] progresses = null;
        private bool[] isObjectLoaded;
        private UnityObject[] objects;

        private string[] addresses = null;
        public string[] Addresses {
            get
            {
                return addresses;
            }
            set
            {
                addresses = value;
                if (addresses != null)
                {
                    objects = new UnityObject[addresses.Length];
                    isObjectLoaded = new bool[addresses.Length];
                    progresses = new float[addresses.Length];
                    for (int i = 0; i < addresses.Length; ++i)
                    {
                        isObjectLoaded[i] = false;
                        progresses[i] = 0.0f;
                    }
                }
            }
        }

        private UnityObject[] Objects => objects;
        public UnityObject GetObject(int index) => objects[index];
        public void SetObject(int index, UnityObject uObj)
        {
            objects[index] = uObj;
            singleFinish?.Invoke(Addresses[index], uObj, userData);
        }

        public bool GetIsObjectLoaded(int index) => isObjectLoaded[index];
        public void SetObjectLoaded(int index) => isObjectLoaded[index] = true;
         
        public void SetProgress(int index,float progress)
        {
            if(progresses[index] != progress)
            {
                progresses[index] = progress;

                singleProgress?.Invoke(Addresses[index], progress, userData);
            }
        }

        public bool IsFinish()
        {
            bool result = true;
            foreach (var isLoaded in isObjectLoaded)
            {
                if (!isLoaded)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        public void LoadFinish()
        {
            allProgress?.Invoke(Addresses, progresses, userData);
            allFinish?.Invoke(Addresses, objects, userData);
        }
        
        public long GetKey()
        {
            return uniqueID;
        }
    }
}
