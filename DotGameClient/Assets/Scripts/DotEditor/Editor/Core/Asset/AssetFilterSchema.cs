using DotEditor.Core.Util;
using UnityEngine;

namespace DotEditor.Core.Asset
{
    [CreateAssetMenu(fileName ="asset_filter",menuName ="Asset Schema/Filter")]
    public class AssetFilterSchema :ScriptableObject
    {
        public bool isEnable = true;
        public string folder = "Assets";
        public bool includeSubfolder = true;
        public string fileNameFilterRegex = "";
        public string[] assets = new string[0];

        public AssetFilterResult Execute()
        {
            assets = DirectoryUtil.GetAssetsByFileNameFilter(folder, includeSubfolder, fileNameFilterRegex,new string[]{ ".meta"});

            AssetFilterResult result = new AssetFilterResult();
            result.filterFolder = folder;
            result.assets = assets;
            return result;
        }
    }
}
