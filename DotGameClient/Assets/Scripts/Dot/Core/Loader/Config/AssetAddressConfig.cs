using System.Collections.Generic;
using UnityEngine;

namespace Dot.Core.Loader.Config
{
    public class AssetAddressConfig : ScriptableObject, ISerializationCallbackReceiver
    {
        public static readonly string CONFIG_PATH = "Assets/Configs/asset_address_config.asset";
        public static readonly string CONFIG_ASSET_BUNDLE_NAME = "assetaddressconfig";

        public AssetAddressData[] addressDatas = new AssetAddressData[0];

        private Dictionary<string, AssetAddressData> pathToAssetDic = new Dictionary<string, AssetAddressData>();
        private Dictionary<string, string> addressToPathDic = new Dictionary<string, string>();
        private Dictionary<string, List<string>> labelToPathDic = new Dictionary<string, List<string>>();

        public string[] GetAssetPathByAddress(string[] addresses)
        {
            string[] paths = new string[addresses.Length];
            for(int i =0;i<addresses.Length;++i)
            {
                paths[i] = GetAssetPathByAddress(addresses[i]);
            }
            return paths;
        }

        public string GetAssetPathByAddress(string address)
        {
            if(addressToPathDic.TryGetValue(address,out string path))
            {
                return path;
            }
            return null;
        }

        public string[] GetAssetPathByLabel(string label)
        {
            if(labelToPathDic.TryGetValue(label,out List<string> paths))
            {
                return paths.ToArray();
            }
            return null;
        }

        public string GetBundlePathByPath(string path)
        {
            if(pathToAssetDic.TryGetValue(path,out AssetAddressData data))
            {
                return data.bundlePath;
            }
            return null;
        }

        public string[] GetBundlePathByPath(string[] paths)
        {
            string[] bundlePaths = new string[paths.Length];
            for(int i =0;i<paths.Length;++i)
            {
                bundlePaths[i] = GetBundlePathByPath(paths[i]);
            }
            return bundlePaths;
        }
        
        public void OnAfterDeserialize()
        {
            foreach(var data in addressDatas)
            {
                pathToAssetDic.Add(data.assetPath, data);
                addressToPathDic.Add(data.assetAddress, data.assetPath);
                if(data.labels!=null && data.labels.Length>0)
                {
                    foreach(var label in data.labels)
                    {
                        if(!labelToPathDic.TryGetValue(label,out List<string> paths))
                        {
                            paths = new List<string>();
                            labelToPathDic.Add(label, paths);
                        }
                        paths.Add(data.assetPath);
                    }
                }
            }
        }

        public void OnBeforeSerialize()
        {
            
        }
    }
}
