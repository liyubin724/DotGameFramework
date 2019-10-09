using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityObject = UnityEngine.Object;
using SystemObject = System.Object;


namespace DotEditor.Core.Util
{
    public static class AssetDatabaseUtil
    {
        public static string[] FindScenes()
        {
            return GetAssetPathByGUID(AssetDatabase.FindAssets("t:scene"));
        }

        public static string[] FindAssetWithBundleName()
        {
            return GetAssetPathByGUID(AssetDatabase.FindAssets("b:"));
        }

        public static string[] FindAssetInFolder<T>(string folderPath)
        {
            return GetAssetPathByGUID(AssetDatabase.FindAssets($"t:{typeof(T).Name}",new string[] { folderPath }));
        }

        public static string[] FindAssets<T>() where T:UnityEngine.Object
        {
            return GetAssetPathByGUID(AssetDatabase.FindAssets($"t:{typeof(T).Name} "));
        }

        public static string[] FindAssets(string label)
        {
            return GetAssetPathByGUID(AssetDatabase.FindAssets($"l:{label} "));
        }

        public static string[] FindAssets<T>(string label) where T: UnityEngine.Object
        {
            return GetAssetPathByGUID(AssetDatabase.FindAssets($"t:{typeof(T).Name} l:{label}"));
        }

        private static string[] GetAssetPathByGUID(string[] guids)
        {
            if(guids == null)
            {
                return null;
            }

            string[] paths = new string[guids.Length];
            for(int i =0;i<guids.Length;i++)
            {
                paths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
            }
            return paths;
        }

        public static long GetTextureStorageSize(Texture texture)
        {
            if (texture == null)
                return 0;

            var TextureUtilType = Assembly.Load("UnityEditor.dll").GetType("UnityEditor.TextureUtil");
            MethodInfo methodInfo = TextureUtilType.GetMethod("GetStorageMemorySizeLong", BindingFlags.Static | BindingFlags.Public);

            return (long)methodInfo.Invoke(null, new SystemObject[] { texture });
        }

        public static long GetTextureStorageSize(string assetPath)
        {
            Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(assetPath);
            return GetTextureStorageSize(texture);
        }

        public static long GetAssetRuntimeMemorySize(UnityObject uObj)
        {
            return Profiler.GetRuntimeMemorySizeLong(uObj);
        }

        public static long GetAssetRuntimeMemorySize(string assetPath)
        {
            UnityObject uObj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityObject));
            return Profiler.GetRuntimeMemorySizeLong(uObj);
        }
    }
}
