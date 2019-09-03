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
        public bool isGroup;
        public string groupName;

        public int detailDataIndex = 0;
        public AssetDetailData detailData;

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
            Reload();
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return false;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var item = (TreeViewItem<TreeElementWithData<AssetDetailGroupTreeData>>)args.item;
            TreeElementWithData<AssetDetailGroupTreeData> element = item.data;

            Rect contentRect = args.rowRect;
            contentRect.x += GetContentIndent(item);
            contentRect.width -= GetContentIndent(item);

            if (element.Data.isGroup)
            {
                EditorGUI.LabelField(contentRect, new GUIContent(element.Data.groupName));
            }else
            {
                GUILayout.BeginArea(contentRect);
                {
                    GUILayout.BeginHorizontal();
                    {
                        EditorGUIUtil.BeginSetLabelWidth(60);
                        {
                            EditorGUILayout.LabelField(new GUIContent("" + element.Data.detailDataIndex), GUILayout.Width(20));
                            EditorGUILayout.TextField("address:",element.Data.detailData.address);
                            EditorGUILayout.TextField("path:",element.Data.detailData.path);
                            EditorGUILayout.TextField("bundle:",element.Data.detailData.bundle);
                            EditorGUILayout.TextField("labels:",string.Join(",", element.Data.detailData.labels),GUILayout.MaxWidth(180));
                        }
                        EditorGUIUtil.EndSetLableWidth();

                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndArea();
            }
        }
    }
}
