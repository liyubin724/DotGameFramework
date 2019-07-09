using System.IO;
using XLua;

namespace Dot.XLuaEx
{
    public static class LuaScriptLoader
    {
        private static string[] scriptFormatPath = new string[0];
        public static void SetScriptLoadFromFile(LuaEnv luaEnv,string[] paths)
        {
            scriptFormatPath = paths;
            luaEnv.AddLoader(LoadScriptFromFile);
        }

        private static byte[] LoadScriptFromFile(ref string filePath)
        {
            if(scriptFormatPath!=null && scriptFormatPath.Length>0)
            {
                foreach(var f in scriptFormatPath)
                {
                    string fp = string.Format(f, filePath);
                    if(File.Exists(fp))
                    {
                        filePath = fp;
                        return File.ReadAllBytes(fp);
                    }
                }
            }
            return null;
        }
    }
}
