using DotEditor.Core.Packer;
using DotEditor.Core.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

namespace DotEditor.Core.BundleDepend
{
    public class AssetDependFinder
    {
        public Action<string, float> progressCallback;
        private int findIndex = 0;


        List<string> bundleAssetPathList = null;
        Dictionary<string, string> spriteInAtlasDic = new Dictionary<string, string>();
        Dictionary<string, List<string>> bundleDependAssetDic = new Dictionary<string, List<string>>();
        Dictionary<string, int> assetUsedCountDic = new Dictionary<string, int>();


        public void Find(AssetBundleTagConfig tagConfig)
        {
            bundleAssetPathList = (from groupData in tagConfig.groupDatas
                                   from detailData in groupData.assetDatas
                                   select detailData.assetPath).ToList();
            findIndex = 0;
            DealWithAtlas();

            bundleAssetPathList.ForEach((path) =>
            {
                if(!(Path.GetExtension(path).ToLower() == ".spriteatlas"))
                {

                    progressCallback?.Invoke(path, findIndex / (float)bundleAssetPathList.Count);
                    findIndex++;

                    List<string> depends = new List<string>();
                    FindAssetDependExcludeBundle(path, depends, new string[] { ".cs" , ".shader"});
                    bundleDependAssetDic.Add(path,depends);
                }
            });

            foreach(var kvp in bundleDependAssetDic)
            {
                foreach(var path in kvp.Value)
                {
                    if(assetUsedCountDic.ContainsKey(path))
                    {
                        assetUsedCountDic[path]++;
                    }else
                    {
                        assetUsedCountDic.Add(path, 1);
                    }
                }
            }
        }

        public Dictionary<string,int> GetRepeatUsedAssets()
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            foreach(var kvp in assetUsedCountDic)
            {
                if(kvp.Value>1)
                {
                    result.Add(kvp.Key, kvp.Value);
                }
            }
            return result;
        }

        public string[] GetBundleByUsedAsset(string assetPath)
        {
            List<string> result = new List<string>();
            foreach (var kvp in bundleDependAssetDic)
            {
                if(kvp.Value.IndexOf(assetPath)>0)
                {
                    result.Add(kvp.Key);
                }
            }
            return result.ToArray();
        }

        private void FindAssetDependExcludeBundle(string assetPath,List<string> depends, string[] excludeExtension)
        {
            string[] directDepends = AssetDatabase.GetDependencies(assetPath, false);
            foreach(var path in directDepends)
            {
                if(path.StartsWith("Packages/"))
                {
                    continue;
                }
                string ext = Path.GetExtension(path).ToLower();
                
                if(path!=assetPath && Array.IndexOf(excludeExtension,ext) < 0 && bundleAssetPathList.IndexOf(path)<0 && 
                    depends.IndexOf(path)<0 && !spriteInAtlasDic.ContainsKey(path))
                {
                    depends.Add(path);

                    FindAssetDependExcludeBundle(path,depends,excludeExtension);
                }
            }
        }

        private void DealWithAtlas()
        {
            List<string> atlasPaths = (from path in bundleAssetPathList
                                       where Path.GetExtension(path).ToLower() == ".spriteatlas"
                                       select path).ToList();
            atlasPaths.ForEach((atlasPath) =>
            {
                progressCallback?.Invoke(atlasPath, findIndex / (float)bundleAssetPathList.Count);
                findIndex++;

                SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);
                if (atlas == null)
                {
                    Debug.LogError("AssetDependFinder::Find->atlas is null. path = " + atlasPath);
                }
                else
                {
                    string[] spriteInAtlas = SpriteAtlasUtil.GetDependAssets(atlas);

                    bundleDependAssetDic.Add(atlasPath, new List<string>(spriteInAtlas));

                    foreach (var spritePath in spriteInAtlas)
                    {
                        spriteInAtlasDic.Add(spritePath, atlasPath);
                    }
                }
            });
        }
        
    }
}
