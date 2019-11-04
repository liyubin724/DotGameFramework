using DotEditor.Core.EGUI;
using DotEditor.Core.EGUI.TreeGUI;
using DotEditor.Core.Packer;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace DotEditor.Core.BundleDepend
{
    public class DependTreeData
    {
        public string assetPath;
        public int repeatCount = 0;

        public bool isBundle = false;

        public static DependTreeData Root
        {
            get { return new DependTreeData(); }
        }
    }

    public class AssetDependWindow : EditorWindow
    {
        [MenuItem("Game/Asset Bundle/Bundle Depend Window")]
        public static void ShowWin()
        {
            AssetDependWindow win = EditorWindow.GetWindow<AssetDependWindow>();
            win.titleContent = new GUIContent("Bundle Depend");
            win.Show();
        }

        AssetDependFinder finder = null;
        private void OnEnable()
        {
            AssetBundleTagConfig tagConfig = Util.FileUtil.ReadFromBinary<AssetBundleTagConfig>(BundlePackUtil.GetTagConfigPath());
            finder = BundlePackUtil.CreateAssetDependFinder(tagConfig, true);
        }

        private GUIStyle titleStyle = null;
        private Vector2 scrollPos = Vector2.zero;
        private AssetDependTreeView dependTreeView;
        private TreeViewState dependTreeViewState;

        private void OnGUI()
        {
            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(EditorStyles.label);
                titleStyle.alignment = TextAnchor.MiddleCenter;
                titleStyle.fontSize = 24;
                titleStyle.fontStyle = FontStyle.Bold;
            }

            DrawToolbar();

            EditorGUILayout.LabelField(new GUIContent("Asset Dependencies"), titleStyle, GUILayout.Height(24));
            EditorGUILayout.LabelField(GUIContent.none, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));

            Rect areaRect = GUILayoutUtility.GetLastRect();
            areaRect.x += 1;
            areaRect.width -= 2;
            areaRect.y += 1;
            areaRect.height -= 2;

            EditorGUIUtil.DrawAreaLine(areaRect, Color.blue);

            Rect dependTreeViewRect = areaRect;
            dependTreeViewRect.x += 1;
            dependTreeViewRect.width -= 2;
            dependTreeViewRect.y += 1;
            dependTreeViewRect.height -= 2;
            if (dependTreeView == null)
            {
                InitDependTreeView();
                RefreshDependTreeView();
            }
            dependTreeView?.OnGUI(dependTreeViewRect);
        }

        private void InitDependTreeView()
        {
            dependTreeViewState = new TreeViewState();
            TreeModel<TreeElementWithData<DependTreeData>> data = new TreeModel<TreeElementWithData<DependTreeData>>(
               new List<TreeElementWithData<DependTreeData>>()
               {
                    new TreeElementWithData<DependTreeData>(DependTreeData.Root,"",-1,-1),
               });

            dependTreeView = new AssetDependTreeView(dependTreeViewState,data);
            dependTreeView.dependWin = this;
        }

        private void RefreshDependTreeView()
        {
            TreeModel<TreeElementWithData<DependTreeData>> treeModel = dependTreeView.treeModel;
            TreeElementWithData<DependTreeData> treeModelRoot = treeModel.root;
            treeModelRoot.children?.Clear();

            Dictionary<string, int> repeatAssetDic = finder.GetRepeatUsedAssets();

            List<string> paths = new List<string>();
            paths.AddRange(repeatAssetDic.Keys);
            paths.Sort();

            foreach(var path in paths)
            {
                DependTreeData adData = new DependTreeData();
                adData.assetPath = path;
                adData.repeatCount = repeatAssetDic[path];
                adData.isBundle = false;

                TreeElementWithData<DependTreeData> assetPathTreeData = new TreeElementWithData<DependTreeData>(adData, "", 0, dependTreeView.NextID);
                treeModel.AddElement(assetPathTreeData, treeModelRoot, treeModelRoot.hasChildren ? treeModelRoot.children.Count : 0);

                string[] usedBundles = finder.GetBundleByUsedAsset(path);
                foreach(var bundle in usedBundles)
                {
                    DependTreeData bundleData = new DependTreeData();
                    bundleData.assetPath = bundle;
                    bundleData.isBundle = true;

                    TreeElementWithData<DependTreeData> dependTreeData = new TreeElementWithData<DependTreeData>(bundleData, "", 1, dependTreeView.NextID);
                    treeModel.AddElement(dependTreeData, assetPathTreeData, assetPathTreeData.hasChildren ? assetPathTreeData.children.Count : 0);
                }
            }
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal("toolbar", GUILayout.ExpandWidth(true));
            {
                if (GUILayout.Button("Start Check", "toolbarbutton", GUILayout.Width(100)))
                {
                    //EditorApplication.delayCall += RefreshData;
                }
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
        }
       
    }
}
