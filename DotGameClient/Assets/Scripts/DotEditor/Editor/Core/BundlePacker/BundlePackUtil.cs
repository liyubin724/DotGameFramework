using Dot.Core.Loader;
using Dot.Core.Loader.Config;
using DotEditor.Core.Asset;
using DotEditor.Core.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static DotEditor.Core.Packer.AssetBundlePackConfig;

namespace DotEditor.Core.Packer
{
    public class BundlePackUtil
    {
        public static AssetBundleTagConfig FindOrCreateTagConfig()
        {
            AssetBundleTagConfig config = AssetDatabase.LoadAssetAtPath<AssetBundleTagConfig>(AssetBundleTagConfig.CONFIG_PATH);

            if (config == null)
            {
                config = ScriptableObject.CreateInstance<AssetBundleTagConfig>();
                AssetDatabase.CreateAsset(config, AssetBundleTagConfig.CONFIG_PATH);

                AssetDatabase.ImportAsset(AssetBundleTagConfig.CONFIG_PATH);
            }

             AssetDatabase.SaveAssets();
            return config;
        }

        public static AssetBundlePackConfig FindOrCreatePackConfig()
        {
            AssetBundlePackConfig config = AssetDatabase.LoadAssetAtPath<AssetBundlePackConfig>(AssetBundlePackConfig.CONFIG_PATH);

            if (config == null)
            {
                config = ScriptableObject.CreateInstance<AssetBundlePackConfig>();
                AssetDatabase.CreateAsset(config, AssetBundlePackConfig.CONFIG_PATH);

                AssetDatabase.ImportAsset(AssetBundlePackConfig.CONFIG_PATH);
            }

            AssetDatabase.SaveAssets();
            return config;
        }

        public static void UpdateTagConfigBySchema()
        {
            AssetBundleTagConfig config = AssetDatabase.LoadAssetAtPath<AssetBundleTagConfig>(AssetBundleTagConfig.CONFIG_PATH);
            if (config == null)
            {
                Debug.LogError("AssetBundleSchemaUtil::UpdateTagConfigBySchema->config is null;");
                return;
            }

            string[] settingPaths = AssetDatabaseUtil.FindAssets<AssetBundleSchemaSetting>();
            if (settingPaths == null || settingPaths.Length == 0)
            {
                Debug.LogError("AssetBundleSchemaUtil::UpdateTagConfigBySchema->Not found schema Setting;");
                return;
            }

            AssetBundleSchemaSetting setting = AssetDatabase.LoadAssetAtPath<AssetBundleSchemaSetting>(settingPaths[0]);
            if (setting == null)
            {
                Debug.LogError("AssetBundleSchemaUtil::UpdateTagConfigBySchema->Schema Setting is Null.");
                return;
            }

            AssetBundleSchemaUtil.UpdateTagConfigBySchema(config, setting);
            CreateAssetInBundleConfig(config);
        }

        public static void SetAssetBundleNames(bool isShowProgressBar = false)
        {
            AssetBundleTagConfig config = AssetDatabase.LoadAssetAtPath<AssetBundleTagConfig>(AssetBundleTagConfig.CONFIG_PATH);
            if (config == null)
            {
                Debug.LogError("AssetBundleSchemaUtil::SetAssetBundleNames->config is null;");
                return;
            }

            AssetImporter assetImporter = AssetImporter.GetAtPath(AssetAddressConfig.CONFIG_PATH);
            assetImporter.assetBundleName = AssetAddressConfig.CONFIG_ASSET_BUNDLE_NAME;

            AssetBundleSchemaUtil.SetAssetBundleNames(config, isShowProgressBar);
        }

        public static void CreateAssetInBundleConfig(AssetBundleTagConfig tagConfig)
        {
            AssetAddressConfig config = AssetDatabase.LoadAssetAtPath<AssetAddressConfig>(AssetAddressConfig.CONFIG_PATH);
            if(config==null)
            {
                config = ScriptableObject.CreateInstance<AssetAddressConfig>();
                AssetDatabase.CreateAsset(config, AssetAddressConfig.CONFIG_PATH);
                AssetDatabase.ImportAsset(AssetAddressConfig.CONFIG_PATH);
            }

            AssetBundleAssetData[] datas = (from groupData in tagConfig.groupDatas
                                            where groupData.isMain == true
                                            from assetData in groupData.assetDatas
                                            select assetData).ToArray();

            List<AssetAddressData> addressDatas = new List<AssetAddressData>();
            foreach(var assetData in datas)
            {
                AssetAddressData addressData = new AssetAddressData()
                {
                    assetAddress = assetData.address,
                    assetPath = assetData.path,
                    bundlePath = assetData.bundle,
                };
                if(assetData.labels!=null && assetData.labels.Length>0)
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

        public static void ClearAssetBundleNames(bool isShowProgressBar = false)
        {
            string[] assetPaths = AssetDatabaseUtil.FindAssetWithBundleName();
            if (assetPaths != null && assetPaths.Length > 0)
            {
                EditorUtility.DisplayProgressBar("Clear Asset Bundle Names", "", 0.0f);
                for(int i =0;i<assetPaths.Length;i++)
                {
                    EditorUtility.DisplayProgressBar("Clear Asset Bundle Names", assetPaths[i], i/(float)assetPaths.Length);
                    AssetImporter assetImporter = AssetImporter.GetAtPath(assetPaths[i]);
                    assetImporter.assetBundleName = "";
                }
                EditorUtility.ClearProgressBar();
            }

            AssetDatabase.RemoveUnusedAssetBundleNames();

            AssetDatabase.SaveAssets();
        }

        public static void PackAssetBundle(AssetBundlePackConfig packConfig)
        {
            string targetFolderName = packConfig.buildTarget.ToString();
            string outputTargetDir = packConfig.bundleOutputDir + "/" + targetFolderName + "/" + AssetBundleConst.ASSETBUNDLE_MAINFEST_NAME;

            BuildTarget buildTarget = BuildTarget.NoTarget;
            if (packConfig.buildTarget == BundleBuildTarget.StandaloneWindows64)
            {
                buildTarget = BuildTarget.StandaloneWindows64;
            }
            else if (packConfig.buildTarget == BundleBuildTarget.PS4)
            {
                buildTarget = BuildTarget.PS4;
            }
            else if (packConfig.buildTarget == BundleBuildTarget.XBoxOne)
            {
                buildTarget = BuildTarget.XboxOne;
            }

            PackAsssetBundle(outputTargetDir, packConfig.cleanupBeforeBuild, packConfig.bundleOptions, buildTarget, true);
        }

        public static void PackAsssetBundle(string outputDir,bool isClean,BuildAssetBundleOptions options, BuildTarget buildTarget, bool isShowProgress = false)
        {
            if(isClean && Directory.Exists(outputDir))
            {
                Directory.Delete(outputDir,true);
            }
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }
            
            UpdateTagConfigBySchema();
            ClearAssetBundleNames(isShowProgress);
            SetAssetBundleNames(isShowProgress);

            BuildPipeline.BuildAssetBundles(outputDir, options, buildTarget);
        }
    }
}

