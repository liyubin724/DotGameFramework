using CSObjectWrapEditor;
using System.IO;
using UnityEngine;

namespace DotEditor.XLuaEx
{
    public static class GenPathUtil
    {
        private static readonly string XLuaGenPath = "Assets/Scripts/XLuaGen";

        [GenPath]
        public static string GenPath
        {
            get
            {
                DirectoryInfo dInfo = new DirectoryInfo(Application.dataPath);
                string genDirPath = dInfo.Parent.FullName.Replace("\\", "/") + "/" + XLuaGenPath;
                //string genDirPath = dInfo.Parent.Parent.FullName.Replace("\\", "/") + "/"+XLuaGenPath;

                if(!Directory.Exists(genDirPath))
                {
                    Directory.CreateDirectory(genDirPath);
                }
                return genDirPath;
            }
        }
    }
}
