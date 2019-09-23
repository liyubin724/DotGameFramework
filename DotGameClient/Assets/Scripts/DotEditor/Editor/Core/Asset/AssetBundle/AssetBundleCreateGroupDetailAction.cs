using Dot.Core.Loader.Config;
using DotEditor.Core.Packer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static DotEditor.Core.Packer.AssetBundleTagConfig;

namespace DotEditor.Core.Asset
{
    [CreateAssetMenu(fileName = "create_detail_action", menuName = "Asset Schema/Asset Bundle/Create Action")]
    public class AssetBundleCreateGroupDetailAction : BaseActionSchema
    {
        public AssetBundlePackMode packMode = AssetBundlePackMode.Together;
        public int packCount = 0;
        public AssetAddressMode addressMode = AssetAddressMode.FileNameWithoutExtension;
        public string fileNameFormat = "{0}";
        public string filterFolder = "";
        public string[] labels = new string[0];

        public override AssetExecuteResult Execute(AssetExecuteInput inputData)
        {
            AssetBundleActionInput actionInputData = inputData as AssetBundleActionInput;
            AssetBundleGroupData groupData = actionInputData.groupData;

            string[] assetPaths = (from filterResult in actionInputData.filterResults
                             where string.IsNullOrEmpty(filterFolder) || filterFolder == filterResult.filterFolder let assets = filterResult.assets
                             from asset in assets select asset).ToArray();
            string[] folderPaths = (from filterResult in actionInputData.filterResults
                                    select filterResult.filterFolder).ToArray();
            string rootFolder = "";
            if(folderPaths.Length == 1)
            {
                rootFolder = folderPaths[0];
            }else
            {
                List<string> bundleFolders = null;
                foreach(var folder in folderPaths)
                {
                    List<string> splitList = new List<string>(folder.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries));
                    if (bundleFolders == null) bundleFolders = splitList;
                    else bundleFolders = bundleFolders.Intersect(splitList).ToList();
                }
                rootFolder = string.Join("/", bundleFolders.ToArray());
            }

            for(int i =0;i<assetPaths.Length;++i)
            {
                AssetAddressData assetData = new AssetAddressData();
                assetData.assetAddress = GetAssetAddress(assetPaths[i]);
                assetData.assetPath = assetPaths[i];
                assetData.bundlePath = GetAssetBundle(rootFolder,assetPaths[i]).ToLower();
                assetData.labels = new string[labels.Length];
                Array.Copy(labels, assetData.labels, labels.Length);

                groupData.assetDatas.Add(assetData);
            }
            return null;
        }
        private int groupCount = 0;
        private int groupIndex = 0;
        private string GetAssetBundle(string rootFolder,string assetPath)
        {
            if(packMode == AssetBundlePackMode.Separate)
            {
                char[] replaceChars = new char[] { '.', ' ', '\t' };
                foreach(var c in replaceChars)
                {
                    assetPath = assetPath.Replace(c, '_');
                    
                }
                return assetPath;
            }else if(packMode == AssetBundlePackMode.Together)
            {
                return rootFolder.Replace(' ', '_');
            }else if(packMode == AssetBundlePackMode.GroupByCount)
            {
                groupCount++;
                if (groupCount>=packCount)
                {
                    groupIndex++;
                    groupCount = 0;
                }
                return rootFolder + "_" + groupIndex;
            }
            return null;
        }

        private string GetAssetAddress(string assetPath)
        {
            if (addressMode == AssetAddressMode.FullPath)
                return assetPath;
            else if (addressMode == AssetAddressMode.FileName)
                return Path.GetFileName(assetPath);
            else if (addressMode == AssetAddressMode.FileNameWithoutExtension)
                return Path.GetFileNameWithoutExtension(assetPath);
            else if (addressMode == AssetAddressMode.FileFormatName)
                return string.Format(fileNameFormat, Path.GetFileNameWithoutExtension(assetPath));
            else
                return assetPath;
        }
    }
}
