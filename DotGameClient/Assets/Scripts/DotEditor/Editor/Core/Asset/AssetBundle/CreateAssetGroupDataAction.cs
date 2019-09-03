using Dot.Core.Loader.Config;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SystemObject = System.Object;

namespace DotEditor.Core.Asset
{
    [CreateAssetMenu(fileName = "create_asset_detail_group", menuName = "Asset/Action/Asset Bundle/Create Asset Group Data Action")]
    public class CreateAssetGroupDataAction : BaseActionSchema
    {
        [PropertyOrder(200)]
        [EnumToggleButtons]
        public AssetBundlePackMode packMode = AssetBundlePackMode.Together;

        [ShowIf("IsShowPackCount")]
        public int packCount = 0;

        [PropertyOrder(200)]
        [EnumToggleButtons]
        public AssetAddressMode addressMode = AssetAddressMode.FileNameWithoutExtension;

        [ShowIf("IsShowFileNameFormat")]
        public string fileNameFormat = "{0}";
        
        [PropertyOrder(200)]
        [FolderPath]
        public string filterFolder = "";

        [PropertyOrder(200)]
        public string[] labels = new string[0];

        public override AssetExecuteResult Execute(AssetExecuteInput inputData)
        {
            AssetBundleActionInput actionInputData = inputData as AssetBundleActionInput;
            
            AssetDetailGroupData groupData = new AssetDetailGroupData();
            groupData.groupName = actionInputData.groupName;
            List<AssetDetailData> assetDetailDatas = new List<AssetDetailData>();

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
                AssetDetailData assetDetailData = new AssetDetailData();
                assetDetailData.address = GetAssetAddress(assetPaths[i]);
                assetDetailData.path = assetPaths[i];
                assetDetailData.labels = new string[labels.Length];
                Array.Copy(labels, assetDetailData.labels, labels.Length);
                assetDetailData.bundle = GetAssetBundle(rootFolder,assetPaths[i]).ToLower();
                assetDetailDatas.Add(assetDetailData);
            }
            groupData.assetDetailDatas = assetDetailDatas.ToArray();

            AssetBundleActionResult result = new AssetBundleActionResult();
            result.groupData = groupData;
            return result;
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
        #region odin
        private bool IsShowFileNameFormat() => addressMode == AssetAddressMode.FileFormatName;
        private bool IsShowPackCount() => packMode == AssetBundlePackMode.GroupByCount;
        #endregion
    }
}
