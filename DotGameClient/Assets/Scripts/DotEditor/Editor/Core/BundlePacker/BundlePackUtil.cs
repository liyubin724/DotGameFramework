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
using UnityEngine.U2D;
using UnityEditor.U2D;
using UnityObject = UnityEngine.Object;

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

        public static void UpdateTagConfig()
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
                    (Editor.CreateEditor(aaAssembly) as AssetAddressAssemblyEditor).AutoFindGroup();
                    aaAssembly.Execute();
                }
            }
        }

        public static void UpdateAddressConfig()
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

        /// <summary>
        /// 根据配置中的数据设置BundleName
        /// </summary>
        /// <param name="isShowProgressBar">是否显示进度</param>
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
                    string assetPath = datas[i].assetPath;
                    string bundlePath = datas[i].bundlePath;
                    AssetImporter ai = AssetImporter.GetAtPath(assetPath);
                    ai.assetBundleName = bundlePath;

                    if(Path.GetExtension(assetPath).ToLower() == ".spriteatlas")
                    {
                        SetSpriteBundleNameByAtlas(assetPath, bundlePath);
                    }
                }
            }
            if (isShowProgressBar)
            {
                EditorUtility.ClearProgressBar();
            }

            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// 由于UGUI中SpriteAtlas的特殊性，为了防止UI的Prefab打包无法与Atlas关联，
        /// 从而设定将SpriteAtlas所使用的Sprite一起打包
        /// </summary>
        /// <param name="atlasAssetPath">SpriteAtlas所在的资源路径</param>
        /// <param name="bundlePath">需要设置的BundleName</param>
        private static void SetSpriteBundleNameByAtlas(string atlasAssetPath,string bundlePath)
        {
            SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasAssetPath);
            if (atlas != null)
            {
                List<string> spriteAssetPathList = new List<string>();
                UnityObject[] objs = atlas.GetPackables();
                foreach (var obj in objs)
                {
                    if (obj.GetType() == typeof(Sprite))
                    {
                        spriteAssetPathList.Add(AssetDatabase.GetAssetPath(obj));
                    }
                    else if (obj.GetType() == typeof(DefaultAsset))
                    {
                        string folderPath = AssetDatabase.GetAssetPath(obj);
                        string[] assets = AssetDatabaseUtil.FindAssetInFolder<Sprite>(folderPath);
                        spriteAssetPathList.AddRange(assets);
                    }
                }
                spriteAssetPathList.Distinct();
                foreach (var path in spriteAssetPathList)
                {
                    AssetImporter ai = AssetImporter.GetAtPath(path);
                    ai.assetBundleName = bundlePath;
                }
            }
        }
        
        /// <summary>
        /// 清除设置的BundleName的标签
        /// </summary>
        /// <param name="isShowProgressBar">是否显示清除进度</param>
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

        public static bool AutoPackAssetBundle()
        {
            UpdateTagConfig();
            if(IsAddressRepeat())
            {
                return false;
            }
            UpdateAddressConfig();
            ClearAssetBundleNames();
            SetAssetBundleNames();
            BundlePackConfig packConfig = Util.FileUtil.ReadFromBinary<BundlePackConfig>(BundlePackUtil.GetPackConfigPath());
            PackAssetBundle(packConfig);

            return true;
        }

        public static bool IsAddressRepeat()
        {
            AssetBundleTagConfig tagConfig = Util.FileUtil.ReadFromBinary<AssetBundleTagConfig>(BundlePackUtil.GetTagConfigPath());
            AssetAddressData[] datas = (from groupData in tagConfig.groupDatas
                                        where groupData.isMain
                                        from assetData in groupData.assetDatas
                                        select assetData).ToArray();

            List<string> addressList = new List<string>();
            foreach(var data in datas)
            {
                if(addressList.IndexOf(data.assetAddress)>=0)
                {
                    Debug.LogError("BundlePackUtil::IsAddressRepeat->assetAddress Repeat");
                    return true;
                }else
                {
                    addressList.Add(data.assetAddress);
                }
            }

            return false;
        }
    }
}

