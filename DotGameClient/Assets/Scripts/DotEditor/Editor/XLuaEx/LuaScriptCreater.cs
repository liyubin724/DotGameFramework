using DotEditor.Core.Util;
using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace DotEditor.XLuaEx
{
    internal class CreateLuaScriptAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string content = File.ReadAllText(PathUtil.GetDiskPath(resourceFile));
            string fileName = Path.GetFileNameWithoutExtension(pathName);
            content = content.Replace("#NAME#", fileName);

            string fullName = PathUtil.GetDiskPath(pathName);
            File.WriteAllText(fullName, content);

            AssetDatabase.ImportAsset(pathName);

            Object obj = AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));

            ProjectWindowUtil.ShowCreatedAsset(obj);
        }
    }

    public static class LuaScriptCreater
    {
        private static readonly string LuaScriptIconPath = "Assets/Tools/ScriptTemplates/lua_icon64.png";
        private static readonly string LuaScriptTemplatePath = "Assets/Tools/ScriptTemplates/82-Lua-NewLuaScript.txt";

        [MenuItem("Assets/Create/Lua Script", false, 82)]
        private static void CreateLuaScript()
        {
            if (EditorApplication.isCompiling || EditorApplication.isPlaying)
            {
                EditorUtility.DisplayDialog("警告", "无法在游戏运行时或代码编译时创建lua脚本", "确定");
                return;
            }

            Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(LuaScriptIconPath);
            string scriptDirPath = PathUtil.GetSelectionAssetDirPath();
            
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                    ScriptableObject.CreateInstance<CreateLuaScriptAction>(),
                    scriptDirPath + "/NewLuaScript.txt", icon,
                    LuaScriptTemplatePath);
        }

        
    }
}
