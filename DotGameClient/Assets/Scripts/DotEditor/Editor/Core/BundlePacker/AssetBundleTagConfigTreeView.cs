using Dot.Core.Loader.Config;
using DotEditor.Core.EGUI;
using DotEditor.Core.EGUI.TreeGUI;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace DotEditor.Core.Packer
{
    public class AssetBundleGroupTreeData
    {
        public AssetBundleGroupData groupData;
        public bool isGroup;
        public int dataIndex = -1;

        public bool isAddressRepeat = false;

        public List<AssetAddressData> repeatAddressList = new List<AssetAddressData>();

        public static AssetBundleGroupTreeData Root
        {
            get { return new AssetBundleGroupTreeData(); }
        }
    }

    public class AssetBundleTagConfigTreeView : TreeViewWithTreeModel<TreeElementWithData<AssetBundleGroupTreeData>>
    {
        private GUIContent addressRepeatContent;

        public AssetBundleTagConfigTreeView(TreeViewState state, TreeModel<TreeElementWithData<AssetBundleGroupTreeData>> model) : 
            base(state, model)
        {
            addressRepeatContent = EditorGUIUtility.IconContent("console.erroricon.sml", "Address Repeat");
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
                return rowHeight = EditorGUIUtility.singleLineHeight * 3 + 4;
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
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField(new GUIContent(groupData.groupName+(groupData.isMain?"(Main)":"")));
                        if(groupTreeData.isAddressRepeat)
                        {
                            if(GUILayout.Button(addressRepeatContent))
                            {
                                SetExpanded(args.item.id,true);
                            }
                        }
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    {
                        if (groupTreeData.isAddressRepeat)
                        {
                            if (GUILayout.Button(addressRepeatContent,GUILayout.Width(24)))
                            {
                                Vector2 pos = GUIUtility.GUIToScreenPoint(Input.mousePosition);
                                AssetAddressRepeatPopupWindow.GetWindow().ShowWithParam(groupTreeData.repeatAddressList, pos);
                            }
                        }else
                        {
                            GUILayout.Label(GUIContent.none, GUILayout.Width(24));
                        }

                        EditorGUIUtil.BeginLabelWidth(60);
                        {
                            AssetAddressData assetData = groupData.assetDatas[groupTreeData.dataIndex];
                            EditorGUILayout.LabelField(new GUIContent("" + groupTreeData.dataIndex), GUILayout.Width(20));
                            EditorGUILayout.TextField("address:", assetData.assetAddress);
                            UnityObject uObj = AssetDatabase.LoadAssetAtPath<UnityObject>(assetData.assetPath);
                            EditorGUILayout.ObjectField(GUIContent.none, uObj, typeof(UnityObject), false,GUILayout.Width(120));
                            GUILayout.BeginVertical();
                            {
                                EditorGUILayout.TextField("path:", assetData.assetPath);
                                EditorGUILayout.TextField("bundle:", assetData.bundlePath);
                                EditorGUILayout.TextField("labels:", string.Join(",", assetData.labels));
                            }
                            GUILayout.EndVertical();
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
