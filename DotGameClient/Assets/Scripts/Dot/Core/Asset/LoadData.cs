using System;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Asset
{
    public class LoadData
    {
        public long uniqueID = -1;
        public string[] addresses;
        private bool[] isSingleFinishCalled;
        private UnityObject[] objects;
        public OnAssetLoadFinishCallback singleFinish = null;
        public OnAssetLoadProgressCallback singleProgress = null;
        public OnAssetsLoadFinishCallback allFinish = null;
        public OnAssetsLoadProgressCallback allProgress = null;
        public bool isInstance = false;

        public LoadData()
        {

        }

        public void SetData(string[] addresses,bool isInstance,
            OnAssetLoadFinishCallback singleFinish,
            OnAssetsLoadFinishCallback allFinish,
            OnAssetLoadProgressCallback singleProgress,
            OnAssetsLoadProgressCallback allProgress)
        {
            this.addresses = addresses;
            this.isInstance = isInstance;
            objects = new UnityObject[addresses.Length];
            isSingleFinishCalled = new bool[addresses.Length];
            for(int i =0;i<addresses.Length;++i)
            {
                isSingleFinishCalled[i] = false;
            }
            this.singleFinish = singleFinish;
            this.allFinish = allFinish;
            this.singleProgress = singleProgress;
            this.allProgress = allProgress;
        }


        public UnityObject[] Objects => objects;
        public void SetObject(int index,UnityObject uObj)=> objects[index] = uObj;
        public UnityObject GetObject(int index) => objects[index];
        public void SetIsSingleFinishCalled(int index, bool isCalled) => isSingleFinishCalled[index] = isCalled;
        public bool GetIsSingleFinishCalled(int index) => isSingleFinishCalled[index];

        public void InvokeAssetLoadFinish(string address, UnityObject uObj) => singleFinish?.Invoke(address, uObj);
        public void InvokeAssetLoadProgress(string address, float progress) => singleProgress?.Invoke(address, progress);
        public void InvokeAssetsLoadFinish(UnityObject[] uObjs) => allFinish?.Invoke(addresses, uObjs);
        public void InvokeAssetsLoadProgress(float[] progresses) => allProgress?.Invoke(addresses, progresses);
    }
}
