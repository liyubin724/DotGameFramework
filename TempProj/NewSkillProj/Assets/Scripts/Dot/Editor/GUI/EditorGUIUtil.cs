using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using SystemObject = System.Object;

namespace DotTimeLine
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
    }

    public static class EditorGUIUtil
    {
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
    }
}
