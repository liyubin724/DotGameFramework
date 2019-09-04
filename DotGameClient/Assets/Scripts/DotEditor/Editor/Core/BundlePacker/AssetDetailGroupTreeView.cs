using Dot.Core.Loader.Config;
using DotEditor.Core.EGUI;
using DotEditor.Core.EGUI.TreeGUI;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace DotEditor.Core.Packer
{
    public class AssetDetailGroupTreeData
    {
        public AssetDetailGroupData detailGroupData;
        public bool isGroup;
        public int detailDataIndex = -1;

        public static AssetDetailGroupTreeData Root
        {
            get { return new AssetDetailGroupTreeData(); }
        }
    }

    public class AssetDetailGroupTreeView : TreeViewWithTreeModel<TreeElementWithData<AssetDetailGroupTreeData>>
    {
        public AssetDetailGroupTreeView(TreeViewState state, TreeModel<TreeElementWithData<AssetDetailGroupTreeData>> model) : 
            base(state, model)
        {
            showBorder = true;
            showAlternatingRowBackgrounds = true;
            rowHeight = EditorGUIUtility.singleLineHeight * 2+8;
            Reload();
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
            var item = (TreeViewItem<TreeElementWithData<AssetDetailGroupTreeData>>)args.item;
            AssetDetailGroupTreeData groupTreeData = item.data.Data;

            Rect contentRect = args.rowRect;
            contentRect.x += GetContentIndent(item);
            contentRect.width -= GetContentIndent(item);

            GUILayout.BeginArea(contentRect);
            {
                AssetDetailGroupData detailGroupData = groupTreeData.detailGroupData;
                if(groupTreeData.isGroup)
                {
                    string groupName = detailGroupData.groupName;

                    if (detailGroupData.isMain)
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
                            AssetDetailData detailData = detailGroupData.assetDetailDatas[groupTreeData.detailDataIndex];
                            EditorGUILayout.LabelField(new GUIContent("" + groupTreeData.detailDataIndex), GUILayout.Width(20));
                            EditorGUILayout.TextField("address:", detailData.address);
                            GUILayout.BeginVertical();
                            {
                                EditorGUILayout.TextField("path:", detailData.path);
                                EditorGUILayout.TextField("bundle:", detailData.bundle);
                            }
                            GUILayout.EndVertical();
                            EditorGUILayout.TextField("labels:", string.Join(",", detailData.labels));
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
