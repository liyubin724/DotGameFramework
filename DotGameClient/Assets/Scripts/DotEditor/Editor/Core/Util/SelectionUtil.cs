using System.Collections.Generic;
using UnityEditor;

namespace DotEditor.Core.Util
{
    public static class SelectionUtil
    {
        public static string[] GetSelectionDirs()
        {
            List<string> dirs = new List<string>();
            string[] guids = Selection.assetGUIDs;
            if (guids != null && guids.Length > 0)
            {
                foreach (var guid in guids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    dirs.Add(assetPath);
                }
            }

            return dirs.ToArray();
        }
    }
}
