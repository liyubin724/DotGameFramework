using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Dot.Core.Loader.Config
{
    public class AssetDetailData
    {
        public string address;
        public string path;
        public string bundle;
        public string[] labels = new string[0];
    }

    public class BundleDetailData
    {
        public string path;
        public string md5;
        public uint time33;
    }

    public class AssetBundleDetailData : ScriptableObject
    {
        
    }
}
