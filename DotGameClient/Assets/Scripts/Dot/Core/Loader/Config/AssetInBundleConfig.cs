using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dot.Core.Loader.Config
{
    public class AssetInBundleConfig : ScriptableObject, ISerializationCallbackReceiver
    {
        public static readonly string CONFIG_PATH = "Assets/Configs/asset_in_bundle.asset";
        public static readonly string CONFIG_ASSET_BUNDLE_NAME = "assetinbundleconfig";

        public AssetInBundleData[] datas = new AssetInBundleData[0];
        
        public void OnAfterDeserialize()
        {
            
        }

        public void OnBeforeSerialize()
        {
            
        }
    }
}
