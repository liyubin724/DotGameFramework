using DotEditor.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace DotEditor.Tests
{
    public class TestBundleNameOperation
    {
        [MenuItem("Test/Clear Bundle 1")]
        public static void MenuClearnBundle1()
        {
            string[] assetPaths = GetMatAssets();
            if (assetPaths != null)
            {
                TestClearBundleName1(assetPaths);
            }
        }

        [MenuItem("Test/Clear Bundle 2")]
        public static void MenuClearnBundle2()
        {
            TestClearBundleName2();
        }

        [MenuItem("Test/Set Bundle")]
        public static void MenuSetBundle()
        {
            string[] assetPaths = GetMatAssets();
            if (assetPaths != null)
            {
                TestSetBundleName(assetPaths);
            }
        }

        private static string[] GetMatAssets()
        {
            string[] assetPaths = AssetDatabaseUtil.FindAssets<Material>();
            if (assetPaths != null)
            {
                if(assetPaths.Length>1000)
                {
                    string[] result = new string[1000];
                    Array.Copy(assetPaths, result, 1000);
                    return result;
                }
                return assetPaths;
            }
            return null;
        }


        public static void TestClearBundleName1(string[] assetPaths)
        {
            if (assetPaths!=null)
            {
                DateTime dt = DateTime.Now;

                foreach(var assetPath in assetPaths)
                {
                    AssetImporter ai = AssetImporter.GetAtPath(assetPath);
                    ai.assetBundleName = "";
                }

                AssetDatabase.RemoveUnusedAssetBundleNames();

                TimeSpan ts = DateTime.Now - dt;
                Debug.Log($"TestClearBundleName1->MS:{ts.TotalMilliseconds}");
            }
        }
        public static void TestClearBundleName2()
        {

            DateTime dt = DateTime.Now;
            string[] allNames = AssetDatabase.GetAllAssetBundleNames();
            foreach(var name in allNames)
            {
                AssetDatabase.RemoveAssetBundleName(name, true);
            }

            TimeSpan ts = DateTime.Now - dt;
            Debug.Log($"TestClearBundleName2->MS:{ts.TotalMilliseconds}");
        }


        public static void TestSetBundleName(string[] assetPaths)
        {
            if (assetPaths != null)
            {
                DateTime dt = DateTime.Now;

                foreach (var assetPath in assetPaths)
                {
                    AssetImporter ai = AssetImporter.GetAtPath(assetPath);
                    string bn = assetPath.Replace(' ', '_').Replace('.', '_').ToLower();
                    ai.assetBundleName = bn;
                }

                TimeSpan ts = DateTime.Now - dt;
                Debug.Log($"TestSetBundleName->MS:{ts.TotalMilliseconds}");
            }
        }
    }
}
