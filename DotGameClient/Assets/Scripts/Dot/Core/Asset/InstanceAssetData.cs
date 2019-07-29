using System;
using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Asset
{
    public class InstanceAssetData
    {
        private AssetData assetData = null;
        private List<WeakReference<UnityObject>> instanceWeakRefList = new List<WeakReference<UnityObject>>();

        public InstanceAssetData(AssetData assetData)
        {
            this.assetData = assetData;
            assetData.RetainRefCount();
        }

        public UnityObject GetInstance()
        {
            if(assetData!=null)
            {
                UnityObject uObj = assetData.GetObject();
                if(uObj!=null)
                {
                    UnityObject instance = UnityObject.Instantiate(uObj);
                    instanceWeakRefList.Add(new WeakReference<UnityObject>(instance));
                    return instance;
                }
            }
            return null;
        }

        public bool IsInUsed()
        {
            for(int i =instanceWeakRefList.Count-1;i>=0;--i)
            {
                WeakReference<UnityObject> weakRef = instanceWeakRefList[i];
                if(!weakRef.TryGetTarget(out UnityObject uObj) || uObj == null)
                {
                    instanceWeakRefList.RemoveAt(i);
                }
                else
                {
                    break;
                }
            }
            return instanceWeakRefList.Count > 0;
        }

        public void Release()
        {
            assetData.ReleaseRefCount();
        }
    }
}
