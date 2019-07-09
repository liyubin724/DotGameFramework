using Dot.XLuaEx;
using Sirenix.OdinInspector;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DotEditor.XLuaEx
{
    //[CustomPropertyDrawer(typeof(LuaAsset))]
    public class LuaAssetDrawer : PropertyDrawer
    {
        [DrawWithUnity]
        private TextAsset textAsset = null;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty scriptPathProperty = property.FindPropertyRelative("m_ScriptPath");
            SerializedProperty scriptNameProperty = property.FindPropertyRelative("m_ScriptName");

            if(!string.IsNullOrEmpty(scriptPathProperty.stringValue) && textAsset == null)
            {
                string scriptAssetPath = string.Format("{0}{1}{2}", LuaConfig.LuaAssetDirPath, scriptPathProperty.stringValue,LuaConfig.LuaAssetExtension);
                textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(scriptAssetPath);
            }

            TextAsset newTA = (TextAsset)EditorGUI.ObjectField(position, "Lua Script:", textAsset, typeof(TextAsset), false);
            if(newTA != textAsset)
            {
                textAsset = newTA;
                if(textAsset == null)
                {
                    scriptNameProperty.stringValue = "";
                    scriptPathProperty.stringValue = "";
                }else
                {
                    string assetPath = AssetDatabase.GetAssetPath(textAsset);
                    if(assetPath.StartsWith(LuaConfig.LuaAssetDirPath))
                    {
                        assetPath = assetPath.Replace(LuaConfig.LuaAssetDirPath, "");
                        assetPath = assetPath.Substring(0, assetPath.LastIndexOf(LuaConfig.LuaAssetExtension) );
                        scriptNameProperty.stringValue = Path.GetFileNameWithoutExtension(assetPath);
                        scriptPathProperty.stringValue = assetPath;
                    }else
                    {
                        textAsset = null;
                        scriptNameProperty.stringValue = "";
                        scriptPathProperty.stringValue = "";
                    }
                }
            }
        }
    }
}
