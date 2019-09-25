using DotEditor.Core.Util;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using SystemObject = System.Object;

namespace DotEditor.Core.EGUI
{
    public static class EditorGUIPropertyInfoLayout
    {
        public static void PropertyInfoIntPopField(SystemObject target, PropertyInfo pInfo, int[] popValues)
        {
            string label = pInfo.Name;
            if (!pInfo.CanRead)
            {
                UnityEditor.EditorGUILayout.LabelField(label, "PropertyInfo can't read!");
                return;
            }
            SystemObject value = pInfo.GetValue(target);
            if (value == null)
            {
                UnityEditor.EditorGUILayout.LabelField(label, "Value is Null!");
                return;
            }
            using (new EditorGUI.DisabledScope(!pInfo.CanWrite))
            {
                string[] displayStrs = new string[popValues.Length];
                for(int i =0;i<popValues.Length;i++)
                {
                    displayStrs[i] = popValues[i].ToString();
                }
                
                value = UnityEditor.EditorGUILayout.IntPopup(label, (int)value, displayStrs, popValues);
            }
            if (value != null && pInfo.CanWrite)
            {
                pInfo.SetValue(target, value);
            }
        }

        public static void PropertyInfoIntField(SystemObject target, PropertyInfo pInfo)
        {
            string label = pInfo.Name;
            if (!pInfo.CanRead)
            {
                UnityEditor.EditorGUILayout.LabelField(label, "PropertyInfo can't read!");
                return;
            }
            SystemObject value = pInfo.GetValue(target);
            if (value == null)
            {
                UnityEditor.EditorGUILayout.LabelField(label, "Value is Null!");
                return;
            }
            using (new EditorGUI.DisabledScope(!pInfo.CanWrite))
            {
                value = UnityEditor.EditorGUILayout.IntField(label, (int)value);
            }
            if (value != null && pInfo.CanWrite)
            {
                pInfo.SetValue(target, value);
            }
         }

        public static void PropertyInfoFloatField(SystemObject target, PropertyInfo pInfo)
        {
            string label = pInfo.Name;
            if (!pInfo.CanRead)
            {
                UnityEditor.EditorGUILayout.LabelField(label, "PropertyInfo can't read!");
                return;
            }
            SystemObject value = pInfo.GetValue(target);
            if (value == null)
            {
                UnityEditor.EditorGUILayout.LabelField(label, "Value is Null!");
                return;
            }
            using (new EditorGUI.DisabledScope(!pInfo.CanWrite))
            {
                value = UnityEditor.EditorGUILayout.FloatField(label, (float)value);
            }
            if (value != null && pInfo.CanWrite)
            {
                pInfo.SetValue(target, value);
            }
        }

        public static void PropertyInfoBoolField(SystemObject target, PropertyInfo pInfo)
        {
            string label = pInfo.Name;
            if (!pInfo.CanRead)
            {
                UnityEditor.EditorGUILayout.LabelField(label, "PropertyInfo can't read!");
                return;
            }
            SystemObject value = pInfo.GetValue(target);
            if (value == null)
            {
                UnityEditor.EditorGUILayout.LabelField(label, "Value is Null!");
                return;
            }
            using (new EditorGUI.DisabledScope(!pInfo.CanWrite))
            {
                value = UnityEditor.EditorGUILayout.Toggle(label, (bool)value);
            }
            if (value != null && pInfo.CanWrite)
            {
                pInfo.SetValue(target, value);
            }
        }

        public static void PropertyInfoStringField(SystemObject target,PropertyInfo pInfo)
        {
            string label = pInfo.Name;
            if (!pInfo.CanRead)
            {
                UnityEditor.EditorGUILayout.LabelField(label, "PropertyInfo can't read!");
                return;
            }
            SystemObject value = pInfo.GetValue(target);
            if (value == null)
            {
                UnityEditor.EditorGUILayout.LabelField(label, "Value is Null!");
                return;
            }
            using (new EditorGUI.DisabledScope(!pInfo.CanWrite))
            {
                value = UnityEditor.EditorGUILayout.TextArea(label, (string)value);
            }
            if (value != null && pInfo.CanWrite)
            {
                pInfo.SetValue(target, value);
            }
        }

        public static void PropertyInfoVector3Field(SystemObject target,PropertyInfo pInfo)
        {
            string label = pInfo.Name;
            if (!pInfo.CanRead)
            {
                UnityEditor.EditorGUILayout.LabelField(label, "PropertyInfo can't read!");
                return;
            }
            SystemObject value = pInfo.GetValue(target);
            if (value == null)
            {
                UnityEditor.EditorGUILayout.LabelField(label, "Value is Null!");
                return;
            }
            using (new EditorGUI.DisabledScope(!pInfo.CanWrite))
            {
                value = UnityEditor.EditorGUILayout.Vector3Field(label, (Vector3)value);
            }
            if (value != null && pInfo.CanWrite)
            {
                pInfo.SetValue(target, value);
            }
        }

        public static void PropertyInfoEnumField(SystemObject target,PropertyInfo pInfo)
        {
            string label = pInfo.Name;
            if (!pInfo.CanRead)
            {
                UnityEditor.EditorGUILayout.LabelField(label, "PropertyInfo can't read!");
                return;
            }
            SystemObject value = pInfo.GetValue(target);
            if (value == null)
            {
                UnityEditor.EditorGUILayout.LabelField(label, "Value is Null!");
                return;
            }
            using (new EditorGUI.DisabledScope(!pInfo.CanWrite))
            {
                value = UnityEditor.EditorGUILayout.EnumPopup(label, (Enum)value);
                value = (int)value;
            }
            if (value != null && pInfo.CanWrite)
            {
                pInfo.SetValue(target, value);
            }
        }
    }

    public static class EditorGUILayoutUtil
    {
        public static void PropertyField(SerializedObject sObj,string propertyName)
        {
            SerializedProperty sProperty = sObj.FindProperty(propertyName);
            EditorGUILayout.PropertyField(sProperty);
        }
        
        public static void PropertyInfoField(SystemObject target,PropertyInfo pInfo)
        {
            Type type = pInfo.PropertyType;
            if (type == typeof(Vector3))
            {
                EditorGUIPropertyInfoLayout.PropertyInfoVector3Field(target, pInfo);
            }
            else if (type.IsEnum)
            {
                EditorGUIPropertyInfoLayout.PropertyInfoEnumField(target, pInfo);
            }
            else if (type == typeof(bool))
            {
                EditorGUIPropertyInfoLayout.PropertyInfoBoolField(target, pInfo);
            }
            else if (type == typeof(int))
            {
                EditorGUIPropertyInfoLayout.PropertyInfoIntField(target, pInfo);
            }
            else if (type == typeof(float) || type == typeof(double))
            {
                EditorGUIPropertyInfoLayout.PropertyInfoFloatField(target, pInfo);
            }
            else if (type == typeof(string))
            {
                EditorGUIPropertyInfoLayout.PropertyInfoStringField(target, pInfo);
            }
            else
            {
                UnityEditor.EditorGUILayout.LabelField(pInfo.Name, "Unrecognized type!!");
            }
        }

        public static void DrawAssetFolderSelection(SerializedProperty property,bool isReadonly = true)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUI.BeginDisabledGroup(isReadonly);
                {
                    EditorGUILayout.PropertyField(property);
                }
                EditorGUI.EndDisabledGroup();
                
                if(GUILayout.Button(new GUIContent(EditorGUIUtil.FolderIcon),GUILayout.Width(20),GUILayout.Height(20)))
                {
                    string folderPath = EditorUtility.OpenFolderPanel("folder", property.stringValue, "");
                    if (!string.IsNullOrEmpty(folderPath))
                    {
                        property.stringValue = PathUtil.GetAssetPath(folderPath);
                    }
                }
                if (GUILayout.Button("\u2716", GUILayout.Width(20), GUILayout.Height(20)))
                {
                    property.stringValue = "";
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        public static string DrawAssetFolderSelection(string label,string assetFolder, bool isReadonly = true)
        {
            string folder = assetFolder;
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUI.BeginDisabledGroup(isReadonly);
                {
                    folder = EditorGUILayout.TextField(label, assetFolder);
                }
                EditorGUI.EndDisabledGroup();

                if (GUILayout.Button(new GUIContent(EditorGUIUtil.FolderIcon), GUILayout.Width(20), GUILayout.Height(20)))
                {
                    string folderPath = EditorUtility.OpenFolderPanel("folder", folder, "");
                    if (!string.IsNullOrEmpty(folderPath))
                    {
                        folder = PathUtil.GetAssetPath(folderPath);
                    }
                }
                if (GUILayout.Button("\u2716", GUILayout.Width(20), GUILayout.Height(20)))
                {
                    folder = "";
                }
            }
            EditorGUILayout.EndHorizontal();
            return folder;
        }

        public static string DrawDiskFolderSelection(string label,string diskFolder,bool isReadonly = true)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUI.BeginDisabledGroup(isReadonly);
                {
                    EditorGUILayout.TextField(label,diskFolder);
                }
                EditorGUI.EndDisabledGroup();

                if (GUILayout.Button(new GUIContent(EditorGUIUtil.FolderIcon), GUILayout.Width(20), GUILayout.Height(20)))
                {
                    diskFolder = EditorUtility.OpenFolderPanel("folder", diskFolder, "");
                }
                if (GUILayout.Button("\u2716", GUILayout.Width(20), GUILayout.Height(20)))
                {
                    diskFolder = "";
                }
            }
            EditorGUILayout.EndHorizontal();

            return diskFolder;
        }
    }

    public static class EditorGUIUtil
    {
        private static Texture2D folderIcon = null;
        public static Texture2D FolderIcon
        {
            get
            {
                if (folderIcon == null)
                {
                    folderIcon = EditorGUIUtility.FindTexture("Folder Icon");
                }
                return folderIcon;
            }
        }

        public static void DrawAreaLine(Rect rect, Color color)
        {
            Handles.color = color;

            var points = new Vector3[] {
                new Vector3(rect.x, rect.y, 0),
                new Vector3(rect.x + rect.width, rect.y, 0),
                new Vector3(rect.x + rect.width, rect.y + rect.height, 0),
                new Vector3(rect.x, rect.y + rect.height, 0),
            };

            var indexies = new int[] {
                0, 1, 1, 2, 2, 3, 3, 0,
            };

            Handles.DrawLines(points, indexies);
        }

        private static Stack<float> labelWidthStack = new Stack<float>();
        public static void BeginLabelWidth(float labelWidth)
        {
            labelWidthStack.Push(EditorGUIUtility.labelWidth);
            EditorGUIUtility.labelWidth = labelWidth;
        }

        public static void EndLableWidth()
        {
            if (labelWidthStack.Count > 0)
                EditorGUIUtility.labelWidth = labelWidthStack.Pop();
        }

        private static Stack<Color> guiColorStack = new Stack<Color>();
        public static void BeginGUIColor(Color color)
        {
            guiColorStack.Push(GUI.color);
            GUI.color = color;
        }
        public static void EndGUIColor()
        {
            if (guiColorStack.Count > 0)
                GUI.color = guiColorStack.Pop();
        }

        private static Stack<Color> guiBgColorStack = new Stack<Color>();
        public static void BeginGUIBackgroundColor(Color color)
        {
            guiBgColorStack.Push(GUI.backgroundColor);
            GUI.backgroundColor= color;
        }
        public static void EndGUIBackgroundColor()
        {
            if (guiBgColorStack.Count > 0)
                GUI.backgroundColor = guiBgColorStack.Pop();
        }

        private static Stack<Color> guiContentColorStack = new Stack<Color>();
        public static void BeginGUIContentColor(Color color)
        {
            guiContentColorStack.Push(GUI.contentColor);
            GUI.contentColor = color;
        }
        public static void EndGUIContentColor()
        {
            if (guiContentColorStack.Count > 0)
                GUI.contentColor = guiContentColorStack.Pop();
        }

        public static void BeginIndent()
        {
            EditorGUI.indentLevel++;
        }

        public static void EndIndent()
        {
            EditorGUI.indentLevel--;
        }

        public static string DrawAssetFolderSelection(Rect rect, string label, string assetFolder, bool isReadonly = true)
        {
            string folder = assetFolder;

            EditorGUI.BeginDisabledGroup(isReadonly);
            {
                folder = EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width - 40, rect.height), label, assetFolder);
            }
            EditorGUI.EndDisabledGroup();

            if (GUI.Button(new Rect(rect.x+rect.width - 40, rect.y, 20, rect.height), new GUIContent(EditorGUIUtil.FolderIcon)))
            {
                string folderPath = EditorUtility.OpenFolderPanel("folder", folder, "");
                if (!string.IsNullOrEmpty(folderPath))
                {
                    folder = PathUtil.GetAssetPath(folderPath);
                }
            }
            if (GUI.Button(new Rect(rect.x + rect.width - 20, rect.y, 20, rect.height), "\u2716"))
            {
                folder = "";
            }
            return folder;
        }
    }

    public static class EditorGUIStyle
    {
        public static GUIStyle GetBoldLabelStyle(int fontSize)
        {
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.fontStyle = FontStyle.Bold;
            style.fontSize = fontSize;

            return style;
        }
    }
}
