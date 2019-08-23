using DotEditor.Core.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityObject = UnityEngine.Object;

namespace DotEditor.Core.Asset
{
    public class AssetRefCountData
    {
        public string assetPath;
        public bool isRefChild;
        public int refCount = 0;
        public bool isMain = false;

        public List<string> referrerList = new List<string>();
    }

    public class AssetDependData
    {
        private Dictionary<string, AssetRefCountData> mainAssetRefDic = new Dictionary<string, AssetRefCountData>();
        private Dictionary<string, AssetRefCountData> allAssetRefDic = new Dictionary<string, AssetRefCountData>();

        public void Save()
        {
            StringBuilder dependSB = new StringBuilder();
            foreach (var kvp in allAssetRefDic)
            {
                dependSB.AppendLine("########");

                dependSB.AppendLine($"assetPath={kvp.Value.assetPath}");
                dependSB.AppendLine($"isMain={kvp.Value.isMain}");
                dependSB.AppendLine($"isRefChild={kvp.Value.isRefChild}");
                dependSB.AppendLine($"refCount={ kvp.Value.refCount}");
                if(kvp.Value.referrerList.Count>0)
                {
                    dependSB.Append("referrerList=");
                    foreach(var referrer in kvp.Value.referrerList)
                    {
                        dependSB.Append(referrer + ";");
                    }
                    dependSB.AppendLine();
                }

                dependSB.AppendLine();
            }
            File.WriteAllText("D:/depends.txt", dependSB.ToString());
        }

        public void AddMainAsset(string assetPath,bool isRefChild)
        {
            if(!mainAssetRefDic.ContainsKey(assetPath))
            {
                AssetRefCountData data = new AssetRefCountData();
                data.assetPath = assetPath;
                data.isRefChild = isRefChild;
                data.isMain = true;

                mainAssetRefDic.Add(assetPath, data);
                allAssetRefDic.Add(assetPath, data);
            }
        }

        public void FindMainDependAsset()
        {
            Dictionary<string, List<string>> dependChildDic = new Dictionary<string, List<string>>();
            foreach(var kvp in mainAssetRefDic)
            {
                if(kvp.Value.isRefChild)
                {
                    string[] childDepends = AssetDatabase.GetDependencies(kvp.Key, false);
                    if (childDepends != null && childDepends.Length > 0)
                    {
                        dependChildDic.Add(kvp.Key, new List<string>(childDepends));
                    }
                }
            }
            if (dependChildDic.Count > 0)
            {
                FindDependAsset(dependChildDic);
            }
        }

        private void FindDependAsset(Dictionary<string, List<string>> dependChildDic)
        {
            Dictionary<string, List<string>> nextDependChildDic = new Dictionary<string, List<string>>();
            foreach (var kvp in dependChildDic)
            {
                string referer = kvp.Key;
                List<string> dependList = kvp.Value;
                foreach (var depend in dependList)
                {
                    if (Path.GetExtension(depend) == ".cs")
                    {
                        continue;
                    }

                    if (allAssetRefDic.TryGetValue(depend, out AssetRefCountData data))
                    {
                        data.refCount++;
                        data.referrerList.Add(referer);
                    }
                    else
                    {
                        data = new AssetRefCountData();
                        data.assetPath = depend;
                        data.isRefChild = true;
                        data.refCount++;

                        allAssetRefDic.Add(depend, data);

                        string[] childDepends = AssetDatabase.GetDependencies(depend, false);
                        if (childDepends != null && childDepends.Length > 0)
                        {
                            nextDependChildDic.Add(depend, new List<string>(childDepends));
                        }
                    }
                }

                if (nextDependChildDic.Count > 0)
                {
                    FindDependAsset(nextDependChildDic);
                }
            }
        }

        public string[] GetUsedShader()
        {
            string[] buildInShaderList = GraphicsSettingsUtil.GetAlawysIncludeShaders();
            
            List<string> shaderList = new List<string>();
            foreach(var kvp in allAssetRefDic)
            {
                if(kvp.Value.isMain)
                {
                    continue;
                }

                if(Path.GetExtension(kvp.Key).ToLower() == ".shader" &&!IsInResources(kvp.Key))
                {
                    if(buildInShaderList!=null && Array.IndexOf(buildInShaderList,kvp.Key)>=0)
                    {
                        continue;
                    }
                    shaderList.Add(kvp.Key);
                }
            }

            shaderList.AddRange(GetCgincAssets());

            return shaderList.ToArray();
        }

        private string[] GetCgincAssets()
        {
            string[] textAssets = AssetDatabaseUtil.FindAssets<TextAsset>();
            List<string> cgincList = new List<string>();
            Array.ForEach(textAssets, (textAsset) =>
            {
                if(Path.GetExtension(textAsset).ToLower() == ".cginc" && !IsInResources(textAsset))
                {
                    cgincList.Add(textAsset);
                }
            });

            return cgincList.ToArray();
        }

        private bool IsInResources(string assetPath)
        {
            return assetPath.IndexOf("/Resources/") >= 0;
        }

    }
    
}
