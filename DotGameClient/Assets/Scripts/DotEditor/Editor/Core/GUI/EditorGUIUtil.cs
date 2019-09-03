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

        public static void DrawFolderSelection(SerializedProperty property,bool isReadonly = true)
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
        public static void BeginSetLabelWidth(float labelWidth)
        {
            labelWidthStack.Push(EditorGUIUtility.labelWidth);
            EditorGUIUtility.labelWidth = labelWidth;
        }

        public static void EndSetLableWidth()
        {
            if (labelWidthStack.Count > 0)
                EditorGUIUtility.labelWidth = labelWidthStack.Pop();
        }
    }


}
