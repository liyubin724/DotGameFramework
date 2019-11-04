using DotEditor.Core.EGUI.TreeGUI;
using DotEditor.Core.Util;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace DotEditor.Core.BundleDepend
{
    public class AssetDependTreeView : TreeViewWithTreeModel<TreeElementWithData<DependTreeData>>
    {
        private GUIContent warningIconContent;
        private int curMaxID = 1;
        public int NextID
        {
            get
            {
                return curMaxID++;
            }
        }

        internal AssetDependWindow dependWin = null;

        public AssetDependTreeView(TreeViewState state, TreeModel<TreeElementWithData<DependTreeData>> model) :
            base(state, model)
        {
            warningIconContent = EditorGUIUtility.IconContent("console.warnicon.sml", "Repeat");
            showBorder = true;
            showAlternatingRowBackgrounds = true;
            rowHeight = EditorGUIUtility.singleLineHeight*2;
            Reload();
        }

        //private List<int> cachedExpandIDs = new List<int>();
        //protected override void ExpandedStateChanged()
        //{
        //    List<int> ids = new List<int>(GetExpanded());

        //    IEnumerable<int> collapseIDs = cachedExpandIDs.Except(ids);
        //    foreach(var id in collapseIDs)
        //    {
        //        TreeElementWithData<AssetData> treeViewData = treeModel.Find(id);
        //        if (treeViewData.Data.dependAssets.Count > 0)
        //        {
        //            treeViewData.children?.Clear();
        //            TreeElementWithData<AssetData> dependTreeData = new TreeElementWithData<AssetData>(null, "", treeViewData.depth + 1, NextID);
        //            treeModel.AddElement(dependTreeData, treeViewData, treeViewData.hasChildren ? treeViewData.children.Count : 0);
        //        }
        //    }

        //    IEnumerable<int> expandIDs = ids.Except(cachedExpandIDs);
        //    foreach (var id in expandIDs)
        //    {
        //        TreeElementWithData<AssetData> treeViewData = treeModel.Find(id);
        //        if (treeViewData.Data.dependAssets.Count > 0)
        //        {
        //            treeViewData.children?.Clear();

        //            for (int j = 0; j < treeViewData.Data.dependAssets.Count; ++j)
        //            {
        //                AssetData aData = treeViewData.Data.dependAssets[j];
        //                TreeElementWithData<AssetData> dependTreeData = new TreeElementWithData<AssetData>(aData, "", treeViewData.depth + 1, NextID);
        //                treeModel.AddElement(dependTreeData, treeViewData, treeViewData.hasChildren ? treeViewData.children.Count : 0);

        //                if(aData.dependAssets.Count>0)
        //                {
        //                    TreeElementWithData<AssetData> emptyData = new TreeElementWithData<AssetData>(null, "", treeViewData.depth + 2, NextID);
        //                    treeModel.AddElement(emptyData, dependTreeData, dependTreeData.hasChildren ? dependTreeData.children.Count : 0);
        //                }
        //            }
        //        }
        //    }

        //    cachedExpandIDs = ids;
        //}
        
        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return false;
        }

        protected override bool CanStartDrag(CanStartDragArgs args)
        {
            return false;
        }
        
        protected override void RowGUI(RowGUIArgs args)
        {
            var item = (TreeViewItem<TreeElementWithData<DependTreeData>>)args.item;
            DependTreeData assetData = item.data.Data;

            if (assetData == null)
            {
                return;
            }
            Rect contentRect = args.rowRect;
            contentRect.x += GetContentIndent(item);
            contentRect.width -= GetContentIndent(item);

            Rect rect = contentRect;
            rect.width -= 80;
            
            if(assetData.isBundle)
            {
                EditorGUI.LabelField(rect, assetData.assetPath);
            }else
            {
                EditorGUI.LabelField(rect, assetData.assetPath+$"({assetData.repeatCount})");
            }

            rect.x += rect.width+5;
            rect.width = 70;
            if(GUI.Button(rect,"selected"))
            {
                SelectionUtil.ActiveObject(assetData.assetPath);
            }
        }
    }
}
