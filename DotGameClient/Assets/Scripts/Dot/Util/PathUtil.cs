using UnityEngine;

namespace Dot.Util
{
    public static class PathUtil
    {
        public static string GetAssetPath(string dirPath)
        {
            dirPath = dirPath.Replace("\\", "/");
            if (dirPath.StartsWith(Application.dataPath))
            {
                return "Assets" + dirPath.Replace(Application.dataPath, "");
            }
            return string.Empty;
        }

        public static string GetDiskPath(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return string.Empty;
            }
            assetPath = assetPath.Replace("\\", "/");
            if (!assetPath.StartsWith("Assets"))
            {
                return string.Empty;
            }
            return Application.dataPath + assetPath.Substring(assetPath.IndexOf("Assets") + 6);
        }
    }
}
