using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemObject = System.Object;

namespace DotEditor.Core.Asset
{
    public class AssetBundleDetailAction : BaseActionSchema
    {
        [PropertyOrder(200)]
        [EnumToggleButtons]
        public AssetBundlePackMode packMode = AssetBundlePackMode.Together;

        [PropertyOrder(200)]
        [EnumToggleButtons]
        public AssetAddressMode addressMode = AssetAddressMode.FileNameWithoutExtension;

        [PropertyOrder(200)]
        [FolderPath]
        public string filterFolder = "";

        [PropertyOrder(200)]
        public string[] labels = new string[0];

        public override void Execute(Dictionary<string, object> dataDic)
        {
            AssetFilterResult[] filterResults = dataDic[AssetSchemaConst.ASSET_FILTER_DATA_NAME] as AssetFilterResult[];

            string sameFolderPath = "";
            if(packMode == AssetBundlePackMode.Together)
            {
                var folders = (from result in filterResults select result.filterFolder).ToArray();

            }

            if(filterResults!=null)
            {
                foreach(var filterResult in filterResults)
                {

                }
            }
        }
    }
}
