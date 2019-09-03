using Dot.Core.Loader.Config;
using DotEditor.Core.EGUI.TreeGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private void OnEnable()
        {
            detailConfig = BundlePackUtil.FindOrCreateConfig();
            InitDetailGroupTreeView();
        }

        private void InitDetailGroupTreeView()
        {
            detailGroupTreeViewState = new TreeViewState();
            detailGroupTreeView = new AssetDetailGroupTreeView(detailGroupTreeViewState, GetDetailGroupTreeModel());
        }

        private TreeModel<TreeElementWithData<AssetDetailGroupTreeData>> GetDetailGroupTreeModel()
        {
            TreeModel<TreeElementWithData<AssetDetailGroupTreeData>> data = new TreeModel<TreeElementWithData<AssetDetailGroupTreeData>>(
               new List<TreeElementWithData<AssetDetailGroupTreeData>>()
               {
                    new TreeElementWithData<AssetDetailGroupTreeData>(AssetDetailGroupTreeData.Root,"",-1,-1),
               });

            for(int i =0;i<detailConfig.assetGroupDatas.Count;i++)
            {
                AssetDetailGroupData groupData = detailConfig.assetGroupDatas[i];
                TreeElementWithData<AssetDetailGroupTreeData> groupElementData = new TreeElementWithData<AssetDetailGroupTreeData>(
                    new AssetDetailGroupTreeData()
                    {
                        isGroup = true,
                        groupName = groupData.groupName,
                    },"",0,(i+1)*100);

                data.AddElement(groupElementData,data.root, data.root.hasChildren ? data.root.children.Count : 0);

                for (int j = 0;j<groupData.assetDetailDatas.Length;++j)
                {
                    TreeElementWithData<AssetDetailGroupTreeData> detailElementData = new TreeElementWithData<AssetDetailGroupTreeData>(
                    new AssetDetailGroupTreeData()
                    {
                        isGroup = false,
                        detailDataIndex = j,
                        detailData = groupData.assetDetailDatas[j],
                    }, "", 1, (i + 1) * 100 + (j+1));

                    data.AddElement(detailElementData, groupElementData, groupElementData.hasChildren ? groupElementData.children.Count : 0);
                }

            }

            return data;
        }

        private void OnGUI()
        {
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

            EditorGUILayout.BeginVertical();
            {
                
                EditorGUILayout.BeginHorizontal(GUILayout.Height(40),GUILayout.ExpandHeight(true));
                {
                    if(GUILayout.Button("Update Asset Detail"))
                    {
                        BundlePackUtil.UpdateAssetDetailConfigBySchema();
                        InitDetailGroupTreeView();
                    }
                    if(GUILayout.Button("Set Asset Bundle Names"))
                    {
                        BundlePackUtil.SetAssetBundleNames();
                    }
                    if(GUILayout.Button("Clear Asset Bundle Names"))
                    {
                        BundlePackUtil.ClearAssetBundleNames();
                    }
                }
                EditorGUILayout.EndHorizontal();

                

            }
            EditorGUILayout.EndVertical();
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
        
        private void ExpandOrCollapseTreeView(bool isExpand)
        {
            isExpandAll = isExpand;
            if (isExpandAll)
            {
                detailGroupTreeView.ExpandAll();
            }
            else
            {
                detailGroupTreeView.CollapseAll();
            }
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal("toolbar", GUILayout.ExpandWidth(true));
            {
                if(GUILayout.Button(isExpandAll? "\u25BC" : "\u25BA", "toolbarbutton",GUILayout.Width(60)))
                {
                    ExpandOrCollapseTreeView(!isExpandAll);
                }
                GUILayout.FlexibleSpace();

                selecteddSearchParamIndex = EditorGUILayout.Popup(selecteddSearchParamIndex, SearchParams, "ToolbarDropDown", GUILayout.Width(80));

                EditorGUILayout.LabelField("", GUILayout.Width(200));
                Rect lastRect = GUILayoutUtility.GetLastRect();
                Rect searchFieldRect = new Rect(lastRect.x, lastRect.y, 180, 16);
                string newSearchText = EditorGUI.TextField(searchFieldRect, "", searchText, "toolbarSeachTextField"); ;
                Rect searchCancelRect = new Rect(searchFieldRect.x + searchFieldRect.width, searchFieldRect.y, 16, 16);
                if (GUI.Button(searchCancelRect, "", "ToolbarSeachCancelButton"))
                {
                    newSearchText = "";
                    Repaint();
                }
                if (searchText != newSearchText)
                {
                    searchText = newSearchText;
                    InitDetailGroupTreeView();

                    //if (!string.IsNullOrEmpty(searchText))
                    //{
                    //    ExpandOrCollapseTreeView(true);
                    //}
                }
            }
            EditorGUILayout.EndHorizontal();
        }

    }
}
