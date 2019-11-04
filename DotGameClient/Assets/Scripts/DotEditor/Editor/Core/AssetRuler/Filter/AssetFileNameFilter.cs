using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DotEditor.Core.AssetRuler
{
    [CreateAssetMenu(fileName = "asset_filename_filter", menuName = "Asset Address/Filters/File Name",order =1)]
    public class AssetFileNameFilter : AssetFilter
    {
        public bool ignoreCase = true;
        public string fileNameRegex = "";

        public override bool IsMatch(string assetPath)
        {
            string fileName = Path.GetFileName(assetPath);
            return Regex.IsMatch(fileName, fileNameRegex, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
        }
    }
}
