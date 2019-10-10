using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityObject = UnityEngine.Object;
using SystemObject = System.Object;
using System.IO;

namespace DotEditor.Core.Util
{
    public static class AssetDatabaseUtil
    {
        /// <summary>
        /// 查找所有的场景资源
        /// </summary>
        /// <returns></returns>
        public static string[] FindScenes()
        {
            return GetAssetPathByGUID(AssetDatabase.FindAssets("t:scene"));
        }

        /// <summary>
        /// 查找所有设置过BundleName的所有资源
        /// </summary>
        /// <returns></returns>
        public static string[] FindAssetWithBundleName()
        {
            return GetAssetPathByGUID(AssetDatabase.FindAssets("b:"));
        }

        /// <summary>
        /// 在给定的目录(folderPath)中查找指定类型的资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static string[] FindAssetInFolder<T>(string folderPath)
        {
            return GetAssetPathByGUID(AssetDatabase.FindAssets($"t:{typeof(T).Name}",new string[] { folderPath }));
        }

        /// <summary>
        /// 查找指定类型的资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string[] FindAssets<T>() where T:UnityEngine.Object
        {
            return GetAssetPathByGUID(AssetDatabase.FindAssets($"t:{typeof(T).Name} "));
        }

        /// <summary>
        /// 查找指定标签的资源(label)
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public static string[] FindAssets(string label)
        {
            return GetAssetPathByGUID(AssetDatabase.FindAssets($"l:{label} "));
        }

        /// <summary>
        /// 根据资源的T及设置的标签label查找符合的资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="label"></param>
        /// <returns>符合条件的资源地址的数组</returns>
        public static string[] FindAssets<T>(string label) where T: UnityEngine.Object
        {
            return GetAssetPathByGUID(AssetDatabase.FindAssets($"t:{typeof(T).Name} l:{label}"));
        }

        /// <summary>
        /// 将guid转换成资源路径
        /// 由于使用AssetDatabase.FindAssets得到的资源为guid,为方便使用需要转换一下
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 在Project中创建指定类型的资源
        /// 只能创建ScriptableObject类型的资源，对于给定的名称，如果目录中已经存在相同名称的资源，会进行重命名
        /// 创建资源时可以指定资源存储的目录 (assetFolder基于Unity的Assets目录)
        /// </summary>
        /// <typeparam name="T">资源的类型</typeparam>
        /// <param name="fileName">资源名称</param>
        /// <param name="assetFolder">资源存储目录，默认为空</param>
        /// <returns></returns>
        public static T CreateAsset<T>(string fileName,string assetFolder = "") where T:ScriptableObject
        {
            if (fileName.Contains("/"))
                throw new ArgumentException("Base name should not contain slashes");
            if(!string.IsNullOrEmpty(assetFolder)&&!assetFolder.StartsWith("Assets"))
            {
                throw new ArgumentException("Asset Folder should be start with Assets");
            }

            string folderPath = string.Empty;
            if(!string.IsNullOrEmpty(assetFolder))
            {
                string diskFolderPath = PathUtil.GetDiskPath(assetFolder);
                if(!Directory.Exists(diskFolderPath))
                {
                    Directory.CreateDirectory(diskFolderPath);
                }
                folderPath = assetFolder;
            }else
            {
                folderPath = AssetDatabase.GetAssetPath(Selection.activeObject);

                if (string.IsNullOrEmpty(folderPath))
                {
                    folderPath = "Assets";
                }
                else if (Path.GetExtension(folderPath) != string.Empty)
                {
                    folderPath = folderPath.Replace(Path.GetFileName(folderPath), string.Empty);
                }
            }

            var asset = ScriptableObject.CreateInstance<T>();
            var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(folderPath + "/" + fileName + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            return asset;
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
