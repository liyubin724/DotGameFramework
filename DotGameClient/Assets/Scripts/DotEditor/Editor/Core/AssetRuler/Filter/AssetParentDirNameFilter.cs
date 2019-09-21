using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DotEditor.Core.AssetRuler
{
    [CreateAssetMenu(fileName = "asset_parentdir_filter", menuName = "Asset Ruler/Filter/In Parent Dir", order = 3)]
    public class AssetParentDirNameFilter : AssetFilter
    {
        public bool ignoreCase = true;
        public string dirNameRegex = "";

        public override bool IsMatch(string assetPath)
        {
            FileInfo fi = new FileInfo(assetPath);
            DirectoryInfo di = fi.Directory;

            return Regex.IsMatch(di.Name, dirNameRegex, ignoreCase?RegexOptions.IgnoreCase:RegexOptions.None);
        }
    }
}
