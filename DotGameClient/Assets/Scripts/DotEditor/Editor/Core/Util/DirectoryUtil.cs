using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace DotEditor.Core.Util
{
    public static class DirectoryUtil
    {
        public static string[] GetAsset(string assetDir, bool includeSubdir)
        {
            string diskDir = PathUtil.GetDiskPath(assetDir);
            string[] files = Directory.GetFiles(diskDir, "*.*", includeSubdir ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            if(files!=null && files.Length>0)
            {
                for(int i =0;i<files.Length;i++)
                {
                    files[i] = PathUtil.GetAssetPath(files[i].Replace("\\", "/"));
                }
            }
            return files;
        }

        public static string[] GetAssetsByFileNameFilter(string assetDir,bool includeSubdir,string filter)
        {
            return GetAssetsByFileNameFilter(assetDir, includeSubdir, filter, null);
        }

        public static string[] GetAssetsByFileNameFilter(string assetDir,bool includeSubdir, string filter,string[] ignoreExtersion)
        {
            string[] files = GetAsset(assetDir, includeSubdir);
            List<string> assetPathList = new List<string>();
            foreach(var file in files)
            {
                string fileName = Path.GetFileName(file);
                bool isValid = true;
                if(!string.IsNullOrEmpty(filter))
                {
                    isValid = Regex.IsMatch(fileName, filter);
                }
                if(isValid && ignoreExtersion!=null && ignoreExtersion.Length>0)
                {
                    string fileExt = Path.GetExtension(file).ToLower();
                    if(Array.IndexOf(ignoreExtersion,fileExt)>=0)
                    {
                        isValid = false;
                    }
                }
                if(isValid)
                {
                    assetPathList.Add(file);
                }
            }
            return assetPathList.ToArray();
        }

        /// <summary>
        /// 从sourceDirName目录中复制所有的目录及文件到指定的destDirName目录中
        /// 可以通过ignoreFileExt参数指定忽略的文件后缀。默认采用小写进行比对，忽略大小写
        /// string[] ignoreFileExt = new string[]{".meta"}
        /// </summary>
        /// <param name="sourceDirName">源目录</param>
        /// <param name="destDirName">目标目录</param>
        /// <param name="ignoreFileExt">忽略文件后缀</param>
        private static void Copy(string sourceDirName, string destDirName, string[] ignoreFileExt = null)
        {
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            foreach (string folderPath in Directory.GetDirectories(sourceDirName, "*", SearchOption.AllDirectories))
            {
                if (!Directory.Exists(folderPath.Replace(sourceDirName, destDirName)))
                    Directory.CreateDirectory(folderPath.Replace(sourceDirName, destDirName));
            }

            foreach (string filePath in Directory.GetFiles(sourceDirName, "*.*", SearchOption.AllDirectories))
            {
                var fileDirName = Path.GetDirectoryName(filePath).Replace("\\", "/");

                var fileExt = Path.GetExtension(filePath).ToLower();
                if(ignoreFileExt != null && Array.IndexOf(ignoreFileExt,fileExt)>=0)
                {
                    continue;
                }

                var fileName = Path.GetFileName(filePath);
                string newFilePath = Path.Combine(fileDirName.Replace(sourceDirName, destDirName), fileName);

                File.Copy(filePath, newFilePath, true);
            }
        }
    }
}
