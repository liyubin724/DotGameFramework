using System.Collections.Generic;
using UnityEngine;

namespace Dot.Core.Loader.Config
{
    public class AssetAddressConfig : ScriptableObject, ISerializationCallbackReceiver
    {
        public static readonly string CONFIG_PATH = "Assets/Tools/AssetAddress/asset_address_config.asset";
        public static readonly string CONFIG_ASSET_BUNDLE_NAME = "assetaddressconfig";

        public AssetAddressData[] addressDatas = new AssetAddressData[0];

        private Dictionary<string, AssetAddressData> pathToAssetDic = new Dictionary<string, AssetAddressData>();
        private Dictionary<string, string> addressToPathDic = new Dictionary<string, string>();
        private Dictionary<string, List<string>> labelToPathDic = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> labelToAddressDic = new Dictionary<string, List<string>>();

        public string[] GetAssetPathByAddress(string[] addresses)
        {
            List<string> paths = new List<string>();
            for(int i =0;i<addresses.Length;++i)
            {
                string assetPath = GetAssetPathByAddress(addresses[i]);
                if(assetPath == null)
                {
                    Debug.LogError($"AssetAddressConfig::GetAssetPathByAddress->Not found Asset by Address.address = {addresses[i]}");
                    return null;
                }else
                {
                    paths.Add(assetPath);
                }
            }
            return paths.ToArray() ;
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

        public string[] GetAssetAddressByLabel(string label)
        {
            if (labelToAddressDic.TryGetValue(label, out List<string> addresses))
            {
                return addresses.ToArray();
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
                if(addressToPathDic.ContainsKey(data.assetAddress))
                {
                    Debug.LogError("AssetAddressConfig::OnAfterDeserialize->address repeat.address = "+data.assetAddress);
                    continue;
                }
                addressToPathDic.Add(data.assetAddress, data.assetPath);
                pathToAssetDic.Add(data.assetPath, data);

                if(data.labels!=null && data.labels.Length>0)
                {
                    foreach(var label in data.labels)
                    {
                        if(!labelToPathDic.TryGetValue(label,out List<string> paths))
                        {
                            paths = new List<string>();
                            labelToPathDic.Add(label, paths);
                        }

                        if(!labelToAddressDic.TryGetValue(label,out List<string> addresses))
                        {
                            addresses = new List<string>();
                            labelToAddressDic.Add(label, addresses);
                        }
                        paths.Add(data.assetPath);
                        addresses.Add(data.assetAddress);
                    }
                }
            }
        }

        public void OnBeforeSerialize()
        {
            
        }
    }
}
