using DotEditor.Core.EGUI;
using DotEditor.Core.EGUI.TreeGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace DotEditor.Core.BundleDepend
{
    public class AssetDependTreeView : TreeViewWithTreeModel<TreeElementWithData<AssetData>>
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
        public AssetDependTreeView(TreeViewState state, TreeModel<TreeElementWithData<AssetData>> model) :
            base(state, model)
        {
            warningIconContent = EditorGUIUtility.IconContent("console.warnicon.sml", "Repeat");
            showBorder = true;
            showAlternatingRowBackgrounds = true;
            rowHeight = EditorGUIUtility.singleLineHeight+2;
            Reload();
        }

        private List<int> cachedExpandIDs = new List<int>();
        protected override void ExpandedStateChanged()
        {
            List<int> ids = new List<int>(GetExpanded());

            IEnumerable<int> collapseIDs = cachedExpandIDs.Except(ids);
            foreach(var id in collapseIDs)
            {
                TreeElementWithData<AssetData> treeViewData = treeModel.Find(id);
                if (treeViewData.Data.dependAssets.Count > 0)
                {
                    treeViewData.children?.Clear();
                    TreeElementWithData<AssetData> dependTreeData = new TreeElementWithData<AssetData>(null, "", treeViewData.depth + 1, NextID);
                    treeModel.AddElement(dependTreeData, treeViewData, treeViewData.hasChildren ? treeViewData.children.Count : 0);
                }
            }

            IEnumerable<int> expandIDs = ids.Except(cachedExpandIDs);
            foreach (var id in expandIDs)
            {
                TreeElementWithData<AssetData> treeViewData = treeModel.Find(id);
                if (treeViewData.Data.dependAssets.Count > 0)
                {
                    treeViewData.children?.Clear();

                    for (int j = 0; j < treeViewData.Data.dependAssets.Count; ++j)
                    {
                        AssetData aData = treeViewData.Data.dependAssets[j];
                        TreeElementWithData<AssetData> dependTreeData = new TreeElementWithData<AssetData>(aData, "", treeViewData.depth + 1, NextID);
                        treeModel.AddElement(dependTreeData, treeViewData, treeViewData.hasChildren ? treeViewData.children.Count : 0);

                        if(aData.dependAssets.Count>0)
                        {
                            TreeElementWithData<AssetData> emptyData = new TreeElementWithData<AssetData>(null, "", treeViewData.depth + 2, NextID);
                            treeModel.AddElement(emptyData, dependTreeData, dependTreeData.hasChildren ? dependTreeData.children.Count : 0);
                        }
                    }
                }
            }

            cachedExpandIDs = ids;
        }
        
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
            var item = (TreeViewItem<TreeElementWithData<AssetData>>)args.item;
            AssetData assetData = item.data.Data;

            if (assetData == null)
            {
                return;
            }

            Rect contentRect = args.rowRect;
            contentRect.x += GetContentIndent(item);
            contentRect.width -= GetContentIndent(item);
            contentRect.height = EditorGUIUtility.singleLineHeight;

            Rect iconRect = contentRect;
            iconRect.width = iconRect.height;
            
            if (assetData.IsAssetRepeat())
            {
                EditorGUI.LabelField(iconRect, warningIconContent);
            }

            EditorGUIUtil.BeginLabelWidth(60);
            {
                contentRect.x += contentRect.height;
                contentRect.width = contentRect.width - 280;
                EditorGUI.TextField(contentRect, new GUIContent("Path"), assetData.assetPath);

                contentRect.x += contentRect.width+20;
                contentRect.width = 240;
                UnityEngine.Object uObj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetData.assetPath);
                EditorGUI.ObjectField(contentRect, "Object", uObj, typeof(UnityEngine.Object), false);
            }
            EditorGUIUtil.EndLableWidth();
        }
    }
}
