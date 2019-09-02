using DotEditor.Core.Util;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DotEditor.Core.Asset
{
    [CreateAssetMenu(fileName ="asset_filter_schema",menuName ="Asset/Asset Filter Schema")]
    public class AssetFilterSchema :ScriptableObject
    {
        [PropertyTooltip("是否使用此过滤器")]
        public bool isEnable = true;

        [FolderPath]
        public string folder = "Assets";
        [PropertyTooltip("是否遍历子目录")]
        public bool includeSubfolder = true;
        [PropertyTooltip("文件筛选条件，此值为正则表达式")]
        public string fileNameFilterRegex = "";

        [Space(20)]
        [InfoBox("为了方便查看，临时保存着筛选后的结果。为了保证正确，请在使用前点击下方按钮或者调用Execute方法重新筛选",InfoMessageType.Warning)]
        [LabelText("Assets")]
        [ListDrawerSettings(Expanded =false,ShowIndexLabels =true,IsReadOnly =true,NumberOfItemsPerPage =50)]
        public string[] assets = new string[0];

        [Button(ButtonSizes.Large)]
        public AssetFilterResult Execute()
        {
            assets = DirectoryUtil.GetAssetsByFileNameFilter(folder, includeSubfolder, fileNameFilterRegex,new string[]{ ".meta"});

            AssetFilterResult result = new AssetFilterResult();
            result.filterFolder = folder;
            result.assets = assets;
            return result;
        }
    }
}
