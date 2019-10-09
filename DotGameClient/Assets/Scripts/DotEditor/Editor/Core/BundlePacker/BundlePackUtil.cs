using Dot.Core.Loader;
using Dot.Core.Loader.Config;
using DotEditor.Core.AssetRuler.AssetAddress;
using DotEditor.Core.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DotEditor.Core.Packer
{
    public static class BundlePackUtil
    {
        internal static string GetPackConfigPath()
        {
            var dataPath = Path.GetFullPath(".");
            dataPath = dataPath.Replace("\\", "/");
            dataPath += "/Library/BundlePack/pack_config.data";
            return dataPath;
        }

        internal static string GetTagConfigPath()
        {
            var dataPath = Path.GetFullPath(".");
            dataPath = dataPath.Replace("\\", "/");
            dataPath += "/Library/BundlePack/tag_config.data";
            return dataPath;
        }

        public static void UpdateConfig()
        {
            UpdateTagConfig();
            UpdateAddressConfig();
        }

        private static void UpdateTagConfig()
        {
            string[] settingPaths = AssetDatabaseUtil.FindAssets<AssetAddressAssembly>();
            if (settingPaths == null || settingPaths.Length == 0)
            {
                Debug.LogError("AssetBundleSchemaUtil::UpdateTagConfigBySchema->Not found schema Setting;");
                return;
            }
            foreach(var assetPath in settingPaths)
            {
                AssetAddressAssembly aaAssembly = AssetDatabase.LoadAssetAtPath<AssetAddressAssembly>(assetPath);
                if(aaAssembly!=null)
                {
                    aaAssembly.Execute();
                }
            }
        }

        private static void UpdateAddressConfig()
        {
            AssetBundleTagConfig tagConfig = Util.FileUtil.ReadFromBinary<AssetBundleTagConfig>(BundlePackUtil.GetTagConfigPath());

            AssetAddressConfig config = AssetDatabase.LoadAssetAtPath<AssetAddressConfig>(AssetAddressConfig.CONFIG_PATH);
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<AssetAddressConfig>();
                AssetDatabase.CreateAsset(config, AssetAddressConfig.CONFIG_PATH);
                AssetDatabase.ImportAsset(AssetAddressConfig.CONFIG_PATH);
            }

            AssetAddressData[] datas = (from groupData in tagConfig.groupDatas
                                        where groupData.isMain == true
                                        from assetData in groupData.assetDatas
                                        select assetData).ToArray();

            List<AssetAddressData> addressDatas = new List<AssetAddressData>();
            foreach (var assetData in datas)
            {
                AssetAddressData addressData = new Dot.Core.Loader.Config.AssetAddressData()
                {
                    assetAddress = assetData.assetAddress,
                    assetPath = assetData.assetPath,
                    bundlePath = assetData.bundlePath,
                };
                if (assetData.labels != null && assetData.labels.Length > 0)
                {
                    addressData.labels = new string[assetData.labels.Length];
                    Array.Copy(assetData.labels, addressData.labels, addressData.labels.Length);
                }
                addressDatas.Add(addressData);
            }

            config.addressDatas = addressDatas.ToArray();
            EditorUtility.SetDirty(config);

            AssetDatabase.SaveAssets();
        }

        public static void SetAssetBundleNames(bool isShowProgressBar = false)
        {
            AssetBundleTagConfig tagConfig = Util.FileUtil.ReadFromBinary<AssetBundleTagConfig>(BundlePackUtil.GetTagConfigPath());

            AssetImporter assetImporter = AssetImporter.GetAtPath(AssetAddressConfig.CONFIG_PATH);
            assetImporter.assetBundleName = AssetAddressConfig.CONFIG_ASSET_BUNDLE_NAME;

            AssetAddressData[] datas = (from groupData in tagConfig.groupDatas
                                        from detailData in groupData.assetDatas
                                        select detailData).ToArray();
            if (isShowProgressBar)
            {
                EditorUtility.DisplayProgressBar("Set Bundle Names", "", 0f);
            }

            if (datas != null && datas.Length > 0)
            {
                for (int i = 0; i < datas.Length; i++)
                {
                    if (isShowProgressBar)
                    {
                        EditorUtility.DisplayProgressBar("Set Bundle Names", datas[i].assetPath, i / (float)datas.Length);
                    }
                    AssetImporter ai = AssetImporter.GetAtPath(datas[i].assetPath);
                    ai.assetBundleName = datas[i].bundlePath;
                }
            }
            if (isShowProgressBar)
            {
                EditorUtility.ClearProgressBar();
            }

            AssetDatabase.SaveAssets();
        }
        
        public static void ClearAssetBundleNames(bool isShowProgressBar = false)
        {
            string[] bundleNames = AssetDatabase.GetAllAssetBundleNames();
            if(isShowProgressBar)
            {
                EditorUtility.DisplayProgressBar("Clear Bundle Names", "", 0.0f);
            }
            for (int i = 0; i < bundleNames.Length; i++)
            {
                if (isShowProgressBar)
                {
                    EditorUtility.DisplayProgressBar("Clear Bundle Names", bundleNames[i], i / (float)bundleNames.Length);
                }
                AssetDatabase.RemoveAssetBundleName(bundleNames[i], true);
            }
            if (isShowProgressBar)
            {
                EditorUtility.ClearProgressBar();
            }

            AssetDatabase.SaveAssets();
        }

        public static void PackAssetBundle(BundlePackConfig packConfig)
        {
            PackAssetBundle(packConfig.outputDirPath, packConfig.cleanupBeforeBuild, packConfig.GetBundleOptions(), packConfig.GetBuildTarget());
        }

        public static void PackAssetBundle(string outputDir,bool isClean,BuildAssetBundleOptions options, BuildTarget buildTarget)
        {
            string outputTargetDir = outputDir + "/" + buildTarget.ToString() + "/" + AssetBundleConst.ASSETBUNDLE_MAINFEST_NAME;

            if (isClean && Directory.Exists(outputTargetDir))
            {
                Directory.Delete(outputTargetDir, true);
            }
            if (!Directory.Exists(outputTargetDir))
            {
                Directory.CreateDirectory(outputTargetDir);
            }
            
            BuildPipeline.BuildAssetBundles(outputTargetDir, options, buildTarget);
        }
    }
}

