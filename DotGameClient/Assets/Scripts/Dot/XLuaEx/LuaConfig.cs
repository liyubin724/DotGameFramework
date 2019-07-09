using UnityEngine;

namespace Dot.XLuaEx
{
    public static class LuaConfig
    {
        public static string LuaAssetDirPath = "Assets/Scripts/DotLua/";
        public static string LuaAssetExtension = ".txt";

        public static string LuaDiskDirPath
        {
            get
            {
                return Application.dataPath + LuaAssetDirPath.Substring("Assets".Length);
            }
            set
            {
                LuaAssetDirPath = value;
            }
        }

        public static string LuaPathFormat
        {
            get
            {
                return $"{LuaDiskDirPath}{{0}}{LuaAssetExtension}";
            }
            set
            {
                LuaAssetExtension = value;
            }
        }
    }
}
