using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemObject = System.Object;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Asset
{
    public class AssetAutoReleaseLoader : IDisposable
    {
        public class LoaderData
        {
            public string address;
            public Action<UnityObject> finishCallback;
            public AssetHandle assetHandle;
        }

        private List<LoaderData> loaderDatas = new List<LoaderData>();
        public AssetAutoReleaseLoader()
        {

        }

        ~AssetAutoReleaseLoader()
        {

        }

        public void Dispose()
        {
            
        }

        public void LoadAsset(string address,Action<UnityObject> finishCallback)
        {
            LoaderData loaderData = new LoaderData();
            loaderData.address = address;
            loaderData.finishCallback = finishCallback;

            AssetHandle assetHandle = AssetLoader.GetInstance().LoadAssetAsync(address, OnLoadAssetComplete, null, loaderData);
            loaderData.assetHandle = assetHandle;
        }

        public void InstanceAsset(string address,Action<UnityObject> finishCallback)
        {
            LoaderData loaderData = new LoaderData();
            loaderData.address = address;
            loaderData.finishCallback = finishCallback;

            AssetHandle assetHandle = AssetLoader.GetInstance().InstanceAssetAsync(address, OnLoadAssetComplete, null, loaderData);
            loaderData.assetHandle = assetHandle;
        }

        private void OnLoadAssetComplete(string address,UnityObject uObj,SystemObject userData)
        {
            LoaderData loaderData = userData as LoaderData;
            if(loaderDatas.IndexOf(loaderData)>=0)
            {
                loaderData.finishCallback?.Invoke(uObj);
            }
        }

        public void ReleaseAsset(string address, Action<UnityObject> finishCallback)
        {
            for(int i =loaderDatas.Count -1;i>=0;--i)
            {
                LoaderData loaderData = loaderDatas[i];
                if (loaderData.address == address && loaderData.finishCallback == finishCallback)
                {
                    loaderData.assetHandle.Release();
                    loaderDatas.RemoveAt(i);
                }
            }
        }
    }
}
