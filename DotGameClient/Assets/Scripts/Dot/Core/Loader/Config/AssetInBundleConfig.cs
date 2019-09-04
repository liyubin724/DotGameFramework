using System.Collections.Generic;
using UnityEngine;

namespace Dot.Core.Loader.Config
{
    public class AssetInBundleConfig : ScriptableObject, ISerializationCallbackReceiver
    {
        public static readonly string CONFIG_PATH = "Assets/Configs/asset_in_bundle.asset";
        public static readonly string CONFIG_ASSET_BUNDLE_NAME = "assetinbundleconfig";

        public AssetInBundleData[] datas = new AssetInBundleData[0];

        private Dictionary<string, AssetInBundleData> pathToAssetDic = new Dictionary<string, AssetInBundleData>();
        private Dictionary<string, string> addressToPathDic = new Dictionary<string, string>();
        private Dictionary<string, List<string>> labelToPathDic = new Dictionary<string, List<string>>();

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
            if(pathToAssetDic.TryGetValue(path,out AssetInBundleData data))
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
            foreach(var data in datas)
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
