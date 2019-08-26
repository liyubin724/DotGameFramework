using DotEditor.Core.Util;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DotEditor.Core.Asset
{
    [CreateAssetMenu(fileName ="asset_filter_schema",menuName ="Asset/Asset Filter Schema")]
    public class AssetFilterSchema :ScriptableObject
    {
        public string folder = "Assets";
        public bool includeSubfolder = true;
        public string fileNameFilterRegex = "";

        public bool isEnable = true;

        [Space(20)]
        [InfoBox("为了方便查看，临时保存着筛选后的结果。为了保证正确，请在使用前点击下方按钮或者调用Execute方法重新筛选",InfoMessageType.Warning)]
        [LabelText("Assets")]
        [ListDrawerSettings(Expanded =false,ShowIndexLabels =true,IsReadOnly =true,NumberOfItemsPerPage =50)]
        public string[] assets = new string[0];

        [Button(ButtonSizes.Large)]
        public void Execute()
        {
            assets = DirectoryUtil.GetAssetsByFileNameFilter(folder, includeSubfolder, fileNameFilterRegex,new string[]{ ".meta"});
        }
    }
}
