using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

namespace DotEditor.Core.Util
{
    public static class UnityObjectUtil
    {
        public static string GetHierarchyName(Transform t)
        {
            if (t == null)
                return "";

            var pname = GetHierarchyName(t.parent);
            if (pname != "")
            {
                return pname + "/" + t.gameObject.name;
            }
            return t.gameObject.name;
        }

        public static bool IsMissingScript(GameObject go, out string[] missingPaths, bool isAutoRemove = false)
        {
            List<string> missPathList = new List<string>();
            SerializedObject serializedObject = new SerializedObject(go);
            Component[] components = go.GetComponents<Component>();
            SerializedProperty componentProperty = serializedObject.FindProperty("m_Component");
            for (int i = components.Length - 1; i >= 0; --i)
            {
                if (components[i] == null)
                {
                    missPathList.Add(GetHierarchyName(go.transform));
                    if (isAutoRemove)
                    {
                        componentProperty.DeleteArrayElementAtIndex(i);
                    }
                }
            }

            foreach (Transform transform in go.transform)
            {
                if (IsMissingScript(transform.gameObject, out string[] paths, isAutoRemove))
                {
                    missPathList.AddRange(paths);
                }
            }
            serializedObject.ApplyModifiedProperties();

            missPathList.Distinct();

            if (missPathList.Count > 0)
            {
                missingPaths = missPathList.ToArray();
                return true;
            }
            else
            {
                missingPaths = null;
                return false;
            }
        }

        public static bool IsMissingScript(Scene scene,out string[] missingPaths,bool isAutoRemove = false)
        {
            List<string> missingPathList = new List<string>();
            GameObject[] gos = scene.GetRootGameObjects();
            foreach (var go in gos)
            {
                if (UnityObjectUtil.IsMissingScript(go, out string[] paths, false))
                {
                    missingPathList.AddRange(paths);
                }
            }
            missingPathList.Distinct();

            if(missingPathList.Count>0)
            {
                missingPaths = missingPathList.ToArray();
                return true;
            }else
            {
                missingPaths = null;
                return false;
            }
        }
    }
}
