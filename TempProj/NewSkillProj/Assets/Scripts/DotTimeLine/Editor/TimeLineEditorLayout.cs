using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using SystemObject = System.Object;

namespace DotTimeLine
{
    public static class TimeLineEditorLayout
    {
        public static void PropertyInfoField(SystemObject target,PropertyInfo pInfo)
        {
            string label = pInfo.Name;
            if (!pInfo.CanRead)
            {
                EditorGUILayout.LabelField(label, "PropertyInfo can't read!");
                return;
            }
            SystemObject value = pInfo.GetValue(target);
            if(value == null)
            {
                EditorGUILayout.LabelField(label, "Value is Null!");
                return;
            }
            Type type = pInfo.PropertyType;
            using (new EditorGUI.DisabledScope(!pInfo.CanWrite))
            {
                if (type == typeof(Vector3))
                {
                    value = EditorGUILayout.Vector3Field(label, (Vector3)value);
                }
                else if (type.IsEnum)
                {
                    value = EditorGUILayout.EnumPopup(label, (Enum)value);
                    value = (int)value;
                }
                else if (type == typeof(bool))
                {
                    value = EditorGUILayout.Toggle(label, (bool)value);
                }
                else if (type == typeof(int))
                {
                    value = EditorGUILayout.IntField(label, (int)value);
                }
                else if (type == typeof(float) || type == typeof(double))
                {
                    value = EditorGUILayout.FloatField(label, (float)value);
                }
                else if (type == typeof(string))
                {
                    value = EditorGUILayout.TextArea(label, (string)value);
                }
                else
                {
                    EditorGUILayout.LabelField(label, "Unrecognized type!!");
                }
                if (value != null && pInfo.CanWrite)
                {
                    pInfo.SetValue(target, value);
                }
            }
        }

    }
}
