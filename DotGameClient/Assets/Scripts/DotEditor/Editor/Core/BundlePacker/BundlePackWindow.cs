using Dot.Core.Loader.Config;
using DotEditor.Core.BundleDepend;
using DotEditor.Core.EGUI;
using DotEditor.Core.EGUI.TreeGUI;
using DotEditor.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace DotEditor.Core.Packer
{
    internal enum RunMode
    {
        AssetDatabase,
        AssetBundle,
    }

    public class BundlePackWindow : EditorWindow
    {
        [MenuItem("Game/Asset Bundle/Bundle Pack Window")]
        public static void ShowWin()
        {
            BundlePackWindow win = EditorWindow.GetWindow<BundlePackWindow>();
            win.titleContent = new GUIContent("Bundle Packer");
            win.Show();
        }

        private static readonly string ASSET_BUNDLE_SYMBOL = "ASSET_BUNDLE";

        private AssetBundleTagConfigTreeView detailGroupTreeView;
        private TreeViewState detailGroupTreeViewState;

        private AssetBundleTagConfig tagConfig = null;
        private BundlePackConfigGUI packConfigGUI;

        private void OnEnable()
        {
            tagConfig = Util.FileUtil.ReadFromBinary<AssetBundleTagConfig>(BundlePackUtil.GetTagConfigPath());
            packConfigGUI = new BundlePackConfigGUI();

            if(PlayerSettingsUtil.HasScriptingDefineSymbol(ASSET_BUNDLE_SYMBOL))
            {
                runMode = RunMode.AssetBundle;
            }
        }

        private void InitDetailGroupTreeView()
        {
            detailGroupTreeViewState = new TreeViewState();
            TreeModel<TreeElementWithData<AssetBundleGroupTreeData>> data = new TreeModel<TreeElementWithData<AssetBundleGroupTreeData>>(
               new List<TreeElementWithData<AssetBundleGroupTreeData>>()
               {
                    new TreeElementWithData<AssetBundleGroupTreeData>(AssetBundleGroupTreeData.Root,"",-1,-1),
               });

            detailGroupTreeView = new AssetBundleTagConfigTreeView(detailGroupTreeViewState, data);
        }

        private void FilterTreeModel()
        {
            TreeModel<TreeElementWithData<AssetBundleGroupTreeData>> treeModel = detailGroupTreeView.treeModel;
            TreeElementWithData<AssetBundleGroupTreeData> treeModelRoot = treeModel.root;
            treeModelRoot.children?.Clear();

            List<AssetAddressData> dataList = (from groupData in tagConfig.groupDatas where groupData.isMain
                                        from detailData in groupData.assetDatas
                                        select detailData).ToList();

            for (int i = 0; i < tagConfig.groupDatas.Count; i++)
            {
                AssetBundleGroupData groupData = tagConfig.groupDatas[i];
                TreeElementWithData<AssetBundleGroupTreeData> groupElementData = new TreeElementWithData<AssetBundleGroupTreeData>(
                    new AssetBundleGroupTreeData()
                    {
                        isGroup = true,
                        groupData = groupData,
                    }, "", 0, (i + 1) * 100);

                treeModel.AddElement(groupElementData, treeModelRoot, treeModelRoot.hasChildren ? treeModelRoot.children.Count : 0);

                bool isAddressRepeat = false;
                for (int j = 0; j < groupData.assetDatas.Count; ++j)
                {
                    AssetAddressData detailData = groupData.assetDatas[j];
                    List<AssetAddressData> repeatList = (from data in dataList
                                                         where data.assetAddress == detailData.assetAddress
                                                         select data).ToList();

                    if (FilterAssetDetailData(detailData))
                    {
                        TreeElementWithData<AssetBundleGroupTreeData> elementData = new TreeElementWithData<AssetBundleGroupTreeData>(
                                new AssetBundleGroupTreeData()
                                {
                                    isGroup = false,
                                    dataIndex = j,
                                    groupData = groupData,
                                }, "", 1, (i + 1) * 100 + (j + 1));

                        if (repeatList.Count > 1)
                        {
                            elementData.Data.isAddressRepeat = true;
                            elementData.Data.repeatAddressList = repeatList;
                            if (!isAddressRepeat)
                            {
                                isAddressRepeat = true;
                            }
                        }

                        treeModel.AddElement(elementData, groupElementData, groupElementData.hasChildren ? groupElementData.children.Count : 0);
                    }
                }
                groupElementData.Data.isAddressRepeat = isAddressRepeat;
            }
        }

        private bool FilterAssetDetailData(AssetAddressData detailData)
        {
            if(string.IsNullOrEmpty(searchText))
            {
                return true;
            }

            bool isValid = false;
            if(selecteddSearchParamIndex == 0 || selecteddSearchParamIndex == 1)
            {
                if(!string.IsNullOrEmpty(detailData.assetAddress))
                {
                    isValid = detailData.assetAddress.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                }
            }
            if(!isValid)
            {
                if(selecteddSearchParamIndex == 0 || selecteddSearchParamIndex == 2)
                {
                    if (!string.IsNullOrEmpty(detailData.assetPath))
                    {
                        isValid = detailData.assetPath.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                    }
                }
            }
            if (!isValid)
            {
                if (selecteddSearchParamIndex == 0 || selecteddSearchParamIndex == 3)
                {
                    if (!string.IsNullOrEmpty(detailData.bundlePath))
                    {
                        isValid = detailData.bundlePath.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
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
            DrawToolbar();

            GUIStyle lableStyle = new GUIStyle(EditorStyles.label);
            lableStyle.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.LabelField("Asset Detail Group List", lableStyle, GUILayout.ExpandWidth(true));

            Rect lastRect = EditorGUILayout.GetControlRect(GUILayout.Height(600));
            if (detailGroupTreeView == null)
            {
                InitDetailGroupTreeView();
                FilterTreeModel();
            }
            detailGroupTreeView?.OnGUI(lastRect);

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox,GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true));
                {
                    packConfigGUI.LayoutGUI();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MaxWidth(180),GUILayout.ExpandHeight(true));
                {
                    DrawOperation();
                }
                EditorGUILayout.EndVertical();
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

        private RunMode runMode = RunMode.AssetDatabase;

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
                EditorGUILayout.Space();
                EditorGUI.BeginChangeCheck();
                {
                    EditorGUIUtil.BeginLabelWidth(65);
                    {
                        runMode = (RunMode)EditorGUILayout.EnumPopup("Run Mode:",runMode, EditorStyles.toolbarPopup,GUILayout.Width(180));
                    }
                    EditorGUIUtil.EndLableWidth();
                }
                if(EditorGUI.EndChangeCheck())
                {
                    if(runMode == RunMode.AssetBundle)
                    {
                        PlayerSettingsUtil.AddScriptingDefineSymbol(ASSET_BUNDLE_SYMBOL);
                    }else
                    {
                        PlayerSettingsUtil.RemoveScriptingDefineSymbol(ASSET_BUNDLE_SYMBOL);
                    }
                }

                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Find Auto Group", "toolbarbutton", GUILayout.Width(120)))
                {
                    BundlePackUtil.FindAndAddAutoGroup(true);

                    tagConfig = Util.FileUtil.ReadFromBinary<AssetBundleTagConfig>(BundlePackUtil.GetTagConfigPath());
                    FilterTreeModel();
                }

                if(GUILayout.Button("Remove Auto Group", "toolbarbutton", GUILayout.Width(120)))
                {
                    BundlePackUtil.DeleteAutoGroup();
                    tagConfig = Util.FileUtil.ReadFromBinary<AssetBundleTagConfig>(BundlePackUtil.GetTagConfigPath());
                    FilterTreeModel();
                }

                if (GUILayout.Button("Open Depend Win","toolbarbutton",GUILayout.Width(160)))
                {
                    AssetDependWindow.ShowWin();
                }

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

        private void DrawOperation()
        {
            if (GUILayout.Button("Update Asset Group"))
            {
                BundlePackUtil.UpdateTagConfig();
                if(BundlePackUtil.IsAddressRepeat())
                {
                    EditorUtility.DisplayDialog("Error", "Address Repeat!", "OK");
                }

                tagConfig = Util.FileUtil.ReadFromBinary<AssetBundleTagConfig>(BundlePackUtil.GetTagConfigPath());
                FilterTreeModel();
            }
            if(GUILayout.Button("Update Address Config"))
            {
                BundlePackUtil.UpdateAddressConfig();
            }
            if(GUILayout.Button("Create Key Class"))
            {
                BundlePackUtil.CreateAddressKeyClass();
            }
            if (GUILayout.Button("Clear Asset Bundle Names"))
            {
                BundlePackUtil.ClearAssetBundleNames(true);
            }
            if (GUILayout.Button("Set Asset Bundle Names"))
            {
                BundlePackUtil.SetAssetBundleNames(true);
            }

            GUILayout.FlexibleSpace();

            EditorGUIUtil.BeginGUIBackgroundColor(Color.red);
            {
                if(GUILayout.Button("Pack Without Depends",GUILayout.Height(30)))
                {
                    EditorApplication.delayCall += () =>
                    {
                        if(BundlePackUtil.PackBundle(false,true))
                        {
                            EditorUtility.DisplayDialog("Success", "Pack AssetBundle Success", "OK");
                        }
                    };
                }
                if (GUILayout.Button("Pack With Depends", GUILayout.Height(30)))
                {
                    EditorApplication.delayCall += () =>
                    {
                        if (BundlePackUtil.PackBundle(true,true))
                        {
                            EditorUtility.DisplayDialog("Success", "Pack AssetBundle Success", "OK");
                        }
                    };
                }
            }
            EditorGUIUtil.EndGUIBackgroundColor();
        }
    }
}
