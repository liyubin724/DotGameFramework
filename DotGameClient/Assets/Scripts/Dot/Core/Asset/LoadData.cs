using Dot.Core.Generic;
using System;
using UnityObject = UnityEngine.Object;
using SystemObject = System.Object;

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
                    isSingleFinishCalled = new bool[addresses.Length];
                    for (int i = 0; i < addresses.Length; ++i)
                    {
                        isSingleFinishCalled[i] = false;
                    }
                }
                
            }
        }

        private bool[] isSingleFinishCalled;
        private UnityObject[] objects;

        public LoadData()
        {

        }

        public UnityObject[] Objects => objects;

       

        public void SetObject(int index,UnityObject uObj)=> objects[index] = uObj;
        public UnityObject GetObject(int index) => objects[index];
        public void SetIsSingleFinishCalled(int index, bool isCalled) => isSingleFinishCalled[index] = isCalled;
        public bool GetIsSingleFinishCalled(int index) => isSingleFinishCalled[index];

        public void InvokeAssetLoadFinish(string address, UnityObject uObj) => singleFinish?.Invoke(address, uObj,userData);
        public void InvokeAssetLoadProgress(string address, float progress) => singleProgress?.Invoke(address, progress, userData);
        public void InvokeAssetsLoadFinish(UnityObject[] uObjs) => allFinish?.Invoke(Addresses, uObjs,userData);
        public void InvokeAssetsLoadProgress(float[] progresses) => allProgress?.Invoke(Addresses, progresses, userData);

        public long GetKey()
        {
            return uniqueID;
        }
    }
}
