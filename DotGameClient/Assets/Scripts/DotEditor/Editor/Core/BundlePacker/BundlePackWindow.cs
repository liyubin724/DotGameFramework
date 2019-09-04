using Dot.Core.Loader.Config;
using DotEditor.Core.EGUI.TreeGUI;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace DotEditor.Core.Packer
{
    public class BundlePackWindow : EditorWindow
    {
        [MenuItem("Game/Asset Bundle/Bundle Pack Window")]
        public static void ShowWin()
        {
            BundlePackWindow win = EditorWindow.GetWindow<BundlePackWindow>();
            win.titleContent = new GUIContent("Bundle Packer");
            win.Show();
        }

        private AssetDetailGroupTreeView detailGroupTreeView;
        private TreeViewState detailGroupTreeViewState;

        private AssetDetailConfig detailConfig = null;

        private void InitDetailGroupTreeView()
        {
            detailGroupTreeViewState = new TreeViewState();
            TreeModel<TreeElementWithData<AssetDetailGroupTreeData>> data = new TreeModel<TreeElementWithData<AssetDetailGroupTreeData>>(
               new List<TreeElementWithData<AssetDetailGroupTreeData>>()
               {
                    new TreeElementWithData<AssetDetailGroupTreeData>(AssetDetailGroupTreeData.Root,"",-1,-1),
               });

            detailGroupTreeView = new AssetDetailGroupTreeView(detailGroupTreeViewState, data);
            FilterTreeModel();
            detailGroupTreeView.Reload();
        }

        private void FilterTreeModel()
        {
            TreeModel<TreeElementWithData<AssetDetailGroupTreeData>> treeModel = detailGroupTreeView.treeModel;
            TreeElementWithData<AssetDetailGroupTreeData> treeModelRoot = treeModel.root;
            treeModelRoot.children?.Clear();

            for (int i = 0; i < detailConfig.assetGroupDatas.Count; i++)
            {
                AssetDetailGroupData groupData = detailConfig.assetGroupDatas[i];
                TreeElementWithData<AssetDetailGroupTreeData> groupElementData = new TreeElementWithData<AssetDetailGroupTreeData>(
                    new AssetDetailGroupTreeData()
                    {
                        isGroup = true,
                        detailGroupData = groupData,
                    }, "", 0, (i + 1) * 100);

                treeModel.AddElement(groupElementData, treeModelRoot, treeModelRoot.hasChildren ? treeModelRoot.children.Count : 0);

                for (int j = 0; j < groupData.assetDetailDatas.Count; ++j)
                {
                    AssetDetailData detailData = groupData.assetDetailDatas[j];
                    if(FilterAssetDetailData(detailData))
                    {
                        TreeElementWithData<AssetDetailGroupTreeData> elementData = new TreeElementWithData<AssetDetailGroupTreeData>(
                                new AssetDetailGroupTreeData()
                                {
                                    isGroup = false,
                                    detailDataIndex = j,
                                    detailGroupData = groupData,
                                }, "", 1, (i + 1) * 100 + (j + 1));

                        treeModel.AddElement(elementData, groupElementData, groupElementData.hasChildren ? groupElementData.children.Count : 0);
                    }
                }

            }
            detailGroupTreeView.Reload();
        }

        private bool FilterAssetDetailData(AssetDetailData detailData)
        {
            if(string.IsNullOrEmpty(searchText))
            {
                return true;
            }

            bool isValid = false;
            if(selecteddSearchParamIndex == 0 || selecteddSearchParamIndex == 1)
            {
                if(!string.IsNullOrEmpty(detailData.address))
                {
                    isValid = detailData.address.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                }
            }
            if(!isValid)
            {
                if(selecteddSearchParamIndex == 0 || selecteddSearchParamIndex == 2)
                {
                    if (!string.IsNullOrEmpty(detailData.path))
                    {
                        isValid = detailData.path.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                    }
                }
            }
            if (!isValid)
            {
                if (selecteddSearchParamIndex == 0 || selecteddSearchParamIndex == 3)
                {
                    if (!string.IsNullOrEmpty(detailData.bundle))
                    {
                        isValid = detailData.bundle.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                    }
                }
            }
            if(!isValid)
            {
                string label = string.Join(",", detailData.labels);
                if (selecteddSearchParamIndex == 0 || selecteddSearchParamIndex == 4)
                {
                    if (!string.IsNullOrEmpty(label))
                    {
                        isValid = label.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                    }
                }
            }
            return isValid;
        }

        private void OnGUI()
        {
            if(detailConfig == null)
            {
                detailConfig = BundlePackUtil.FindOrCreateConfig();
            }
            if (detailGroupTreeView == null)
            {
                InitDetailGroupTreeView();
            }

            DrawToolbar();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                GUIStyle lableStyle = new GUIStyle(EditorStyles.label);
                lableStyle.alignment = TextAnchor.MiddleCenter;
                EditorGUILayout.LabelField("Asset Detail Group List", lableStyle, GUILayout.ExpandWidth(true));

                Rect lastRect = EditorGUILayout.GetControlRect(GUILayout.Height(400));

                detailGroupTreeView.OnGUI(lastRect);
            }
            EditorGUILayout.EndVertical();



            EditorGUILayout.BeginHorizontal(GUILayout.ExpandHeight(true));
            {
                if (GUILayout.Button("Update Asset Detail"))
                {
                    BundlePackUtil.UpdateAssetDetailConfigBySchema();
                    detailConfig = BundlePackUtil.FindOrCreateConfig();
                    FilterTreeModel();
                }
                if (GUILayout.Button("Set Asset Bundle Names"))
                {
                    BundlePackUtil.SetAssetBundleNames(true);
                }
                if (GUILayout.Button("Clear Asset Bundle Names"))
                {
                    BundlePackUtil.ClearAssetBundleNames(true);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        public string[] SearchParams = new string[]
        {
            "All",
            "Address",
            "Path",
            "Bundle",
            "Labels",
        };
        private int selecteddSearchParamIndex = 0;
        private string searchText = "";
        private bool isExpandAll = false;
        
        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal("toolbar", GUILayout.ExpandWidth(true));
            {
                if(GUILayout.Button(isExpandAll? "\u25BC" : "\u25BA", "toolbarbutton",GUILayout.Width(60)))
                {
                    isExpandAll = !isExpandAll;
                    if (isExpandAll)
                    {
                        detailGroupTreeView.ExpandAll();
                    }
                    else
                    {
                        detailGroupTreeView.CollapseAll();
                    }
                }
                GUILayout.FlexibleSpace();

                int newSelectedIndex = EditorGUILayout.Popup(selecteddSearchParamIndex, SearchParams, "ToolbarDropDown", GUILayout.Width(80));
                if(newSelectedIndex != selecteddSearchParamIndex)
                {
                    selecteddSearchParamIndex = newSelectedIndex;
                    FilterTreeModel();
                }

                EditorGUILayout.LabelField("", GUILayout.Width(200));
                Rect lastRect = GUILayoutUtility.GetLastRect();
                Rect searchFieldRect = new Rect(lastRect.x, lastRect.y, 180, 16);
                string newSearchText = EditorGUI.TextField(searchFieldRect, "", searchText, "toolbarSeachTextField"); ;
                Rect searchCancelRect = new Rect(searchFieldRect.x + searchFieldRect.width, searchFieldRect.y, 16, 16);
                if (GUI.Button(searchCancelRect, "", "ToolbarSeachCancelButton"))
                {
                    newSearchText = "";
                    GUI.FocusControl("");
                }
                if(newSearchText != searchText)
                {
                    searchText = newSearchText;
                    FilterTreeModel();

                    isExpandAll = true;
                    detailGroupTreeView.ExpandAll();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

    }
}
