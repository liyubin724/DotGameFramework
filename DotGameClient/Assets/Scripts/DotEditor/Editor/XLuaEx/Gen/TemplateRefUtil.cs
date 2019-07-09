using DotEditor.Core.Util;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XLua;

namespace DotEditor.XLuaEx
{
    public class TemplateRefUtil
    {
        private readonly static string TemplateRefPath = "Assets/Tools/XLua/template_ref.asset";
        private readonly static string TemplateDirPath = "Assets/Tools/XLua/Template";

        [MenuItem("Game/XLuaEx/Create TemplateRef",false,1)]
        public static void CreateTemplateRef()
        {
            TemplateRef templateRef = AssetDatabase.LoadAssetAtPath<TemplateRef>(TemplateRefPath);
            if (templateRef != null)
            {
                AssetDatabase.DeleteAsset(TemplateRefPath);
            }
            templateRef = ScriptableObject.CreateInstance<TemplateRef>();

            string[] templateFiles = Directory.GetFiles(PathUtil.GetDiskPath(TemplateDirPath), "*.txt", SearchOption.TopDirectoryOnly);
            if (templateFiles == null || templateFiles.Length == 0)
            {
                Debug.LogError("");
                return;
            }
            foreach (var f in templateFiles)
            {
                string fAssetPath = PathUtil.GetAssetPath(f);
                string fileName = Path.GetFileNameWithoutExtension(fAssetPath);
                fileName = fileName.Substring(0, fileName.IndexOf("."));
                Debug.Log(fileName);

                TextAsset ta = AssetDatabase.LoadAssetAtPath<TextAsset>(fAssetPath);
                FieldInfo fInfo = typeof(TemplateRef).GetField(fileName, BindingFlags.Public | BindingFlags.Instance);
                fInfo.SetValue(templateRef, ta);

            }
            AssetDatabase.CreateAsset(templateRef, TemplateRefPath);
        }

        public static TemplateRef LoadTemplateRef()
        {
            TemplateRef templateRef = AssetDatabase.LoadAssetAtPath<TemplateRef>(TemplateRefPath);
            if(templateRef == null)
            {
                CreateTemplateRef();
                templateRef = AssetDatabase.LoadAssetAtPath<TemplateRef>(TemplateRefPath);
            }
           
            return templateRef;
        }
    }
}
