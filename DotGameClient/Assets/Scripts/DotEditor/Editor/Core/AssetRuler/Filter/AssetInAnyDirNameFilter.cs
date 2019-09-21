using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DotEditor.Core.AssetRuler
{
    [CreateAssetMenu(fileName = "asset_inanydir_filter", menuName = "Asset Ruler/Filter/In Any Dir", order = 2)]
    public class AssetInAnyDirNameFilter : AssetFilter
    {
        public bool ignoreCase = true;
        public string dirNameRegex = "";

        public override bool IsMatch(string assetPath)
        {
            FileInfo fi = new FileInfo(assetPath);
            DirectoryInfo di = fi.Directory;

            string fullDirName = di.FullName;
            string[] dirs = fullDirName.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
            foreach(var dir in dirs)
            {
                if(Regex.IsMatch(dir, dirNameRegex, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
