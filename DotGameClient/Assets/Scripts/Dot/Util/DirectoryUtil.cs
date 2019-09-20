using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Dot.Util
{
    public static class DirectoryUtil
    {
        public static string[] GetAsset(string assetDir, bool includeSubdir)
        {
            string diskDir = PathUtil.GetDiskPath(assetDir);
            string[] files = Directory.GetFiles(diskDir, "*.*", includeSubdir ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            if (files != null && files.Length > 0)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    files[i] = PathUtil.GetAssetPath(files[i].Replace("\\", "/"));
                }
            }
            return files;
        }

        public static string[] GetAssetsByFileNameFilter(string assetDir, bool includeSubdir, string filter)
        {
            return GetAssetsByFileNameFilter(assetDir, includeSubdir, filter, null);
        }

        public static string[] GetAssetsByFileNameFilter(string assetDir, bool includeSubdir, string filter, string[] ignoreExtersion)
        {
            string[] files = GetAsset(assetDir, includeSubdir);
            List<string> assetPathList = new List<string>();
            foreach (var file in files)
            {
                string fileName = Path.GetFileName(file);
                bool isValid = true;
                if (!string.IsNullOrEmpty(filter))
                {
                    isValid = Regex.IsMatch(fileName, filter);
                }
                if (isValid && ignoreExtersion != null && ignoreExtersion.Length > 0)
                {
                    string fileExt = Path.GetExtension(file).ToLower();
                    if (Array.IndexOf(ignoreExtersion, fileExt) >= 0)
                    {
                        isValid = false;
                    }
                }
                if (isValid)
                {
                    assetPathList.Add(file);
                }
            }
            return assetPathList.ToArray();
        }

    }
}
