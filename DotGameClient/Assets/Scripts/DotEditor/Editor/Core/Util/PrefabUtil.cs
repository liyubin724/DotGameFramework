using System;
using System.Reflection;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using SystemObject = System.Object;

namespace DotEditor.Core.Util
{
    public static class PrefabUtil
    {
        private static MethodInfo openPrefabMethod = null;
        private static MethodInfo GetOpenPrefabMethod()
        {
            if (openPrefabMethod != null)
                return openPrefabMethod;

            MethodInfo[] mInfos = typeof(PrefabStageUtility).GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
            foreach (var mi in mInfos)
            {
                if (mi.Name == "OpenPrefab")
                {
                    if (mi.GetParameters().Length == 1)
                    {
                        openPrefabMethod = mi;
                        break;
                    }
                }
            }
            return openPrefabMethod;
        }

        public static void OpenPrefabStage(string assetPath)
        {
            if (!string.IsNullOrEmpty(assetPath) && assetPath.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase))
            {
                MethodInfo methodInfo = GetOpenPrefabMethod();
                if (methodInfo != null)
                {
                    methodInfo.Invoke(null, new SystemObject[] { assetPath });
                }
            }
        }

        public static bool IsPrefab(string assetPath)
        {
            if (!string.IsNullOrEmpty(assetPath) && assetPath.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        public static void ClosePrefabStage()
        {
            //Type managerType = Assembly.Load("UnityEditor.dll").GetType("UnityEditor.SceneManagement.StageNavigationManager");
            //PropertyInfo pInfo = managerType.GetProperty("instance", BindingFlags.Static);

            //var instance = pInfo.GetValue(null);
            //MethodInfo mInfo = managerType.GetMethod("NavigateBack", BindingFlags.Instance | BindingFlags.NonPublic);
            //mInfo.Invoke(instance, new SystemObject[] { 7 });
        }

        public static bool IsMissingNestPrefab(string assetPath)
        {
            if(!IsPrefab(assetPath))
            {
                return false;
            }

            PrefabUtil.OpenPrefabStage(assetPath);

            PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();
            GameObject rootGO = stage.prefabContentsRoot;
            Transform[] transforms = rootGO.GetComponentsInChildren<Transform>();
            foreach (var t in transforms)
            {
                if (t.name.IndexOf("Missing Prefab") >= 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
