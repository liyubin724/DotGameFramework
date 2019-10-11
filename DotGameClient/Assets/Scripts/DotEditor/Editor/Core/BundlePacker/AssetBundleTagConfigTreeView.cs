﻿using Dot.Core.Loader.Config;
using DotEditor.Core.EGUI;
using DotEditor.Core.EGUI.TreeGUI;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using static DotEditor.Core.Packer.AssetBundleTagConfig;

namespace DotEditor.Core.Packer
{
    public class AssetBundleGroupTreeData
    {
        public AssetBundleGroupData groupData;
        public bool isGroup;
        public int dataIndex = -1;

        public static AssetBundleGroupTreeData Root
        {
            get { return new AssetBundleGroupTreeData(); }
        }
    }

    public class AssetBundleTagConfigTreeView : TreeViewWithTreeModel<TreeElementWithData<AssetBundleGroupTreeData>>
    {
        public AssetBundleTagConfigTreeView(TreeViewState state, TreeModel<TreeElementWithData<AssetBundleGroupTreeData>> model) : 
            base(state, model)
        {
            showBorder = true;
            showAlternatingRowBackgrounds = true;
            Reload();
        }

        protected override float GetCustomRowHeight(int row, TreeViewItem item)
        {
            var viewItem = (TreeViewItem<TreeElementWithData<AssetBundleGroupTreeData>>)item;
            AssetBundleGroupTreeData groupTreeData = viewItem.data.Data;
            if(groupTreeData.isGroup)
            {
                return EditorGUIUtility.singleLineHeight + 2;
            }else
            {
                return rowHeight = EditorGUIUtility.singleLineHeight * 2 + 4;
            }
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
            var item = (TreeViewItem<TreeElementWithData<AssetBundleGroupTreeData>>)args.item;
            AssetBundleGroupTreeData groupTreeData = item.data.Data;

            Rect contentRect = args.rowRect;
            contentRect.x += GetContentIndent(item);
            contentRect.width -= GetContentIndent(item);

            GUILayout.BeginArea(contentRect);
            {
                AssetBundleGroupData groupData = groupTreeData.groupData;
                if(groupTreeData.isGroup)
                {
                    string groupName = groupData.groupName;

                    if (groupData.isMain)
                    {
                        groupName += "(Main)";
                    }
                    EditorGUILayout.LabelField(new GUIContent(groupName));
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    {
                        EditorGUIUtil.BeginLabelWidth(60);
                        {
                            AssetAddressData assetData = groupData.assetDatas[groupTreeData.dataIndex];
                            EditorGUILayout.LabelField(new GUIContent("" + groupTreeData.dataIndex), GUILayout.Width(20));
                            EditorGUILayout.TextField("address:", assetData.assetAddress);
                            GUILayout.BeginVertical();
                            {
                                EditorGUILayout.TextField("path:", assetData.assetPath);
                                EditorGUILayout.TextField("bundle:", assetData.bundlePath);
                            }
                            GUILayout.EndVertical();
                            EditorGUILayout.TextField("labels:", string.Join(",", assetData.labels));
                        }
                        EditorGUIUtil.EndLableWidth();
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndArea();
            
        }
    }
}
