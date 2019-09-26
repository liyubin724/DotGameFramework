using Dot.Core.Loader;
using Dot.Core.Loader.Config;
using Dot.Core.Util;
using DotEditor.Core.EGUI;
using Priority_Queue;
using ReflectionMagic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace DotEditor.Core.BundleView
{
    public class BundleViewWindow : EditorWindow
    {
        [MenuItem("Game/Asset Bundle/Bundle View Window")]
        public static void ShowWin()
        {
            BundleViewWindow win = EditorWindow.GetWindow<BundleViewWindow>();
            win.titleContent = new GUIContent("Bundle Packer");
            win.autoRepaintOnSceneChange = true;
            win.Show();
        }

        private Dictionary<string, bool> assetNodeFoldoutDic = new Dictionary<string, bool>();
        private Dictionary<string, bool> bundleNodeFoldoutDic = new Dictionary<string, bool>();

        private AssetBundleLoader bundleLoader = null;
        private Dictionary<string, AssetNode> assetNodeDic = null;
        private Dictionary<string, BundleNode> bundleNodeDic = null;
        private AssetAddressConfig assetAddressConfig = null;
        private AssetBundleManifest assetBundleManifest = null;
        
        private Color32 nodeBGColor = new Color32(95, 158, 160, 255);
        private bool isShowAssetNodeMainBundle = true;
        private bool isShowAssetNodeDependBundle = true;
        private bool isShowLoadingAssetNode = false;

        private bool isShowBundleNodeDirect = true;
        private bool isShowLoadingBundleNode = false;

        private List<AssetNode> assetNodes = new List<AssetNode>();
        private List<BundleNode> bundleNodes = new List<BundleNode>();
        private bool isChanged = true;
        private string searchText = "";

        private string[] ToolbarTitle = new string[] {
            "Asset Node",
            "Bundle Node",
        };

        private int toolbarSelectIndex = 0;
        private SearchField searchField = null;
        
        private Vector2 scrollPos = Vector2.zero;
        private void OnGUI()
        {
            if(!InitLoader())
            {
                return;
            }
            if(searchField == null)
            {
                searchField = new SearchField();
                searchField.autoSetFocusOnFindCommand = true;
            }
            EditorGUILayout.BeginHorizontal("toolbar", GUILayout.ExpandWidth(true));
            {
                int selectedIndex = GUILayout.Toolbar(toolbarSelectIndex, ToolbarTitle, EditorStyles.toolbarButton,GUILayout.MaxWidth(200));
                if(selectedIndex!=toolbarSelectIndex)
                {
                    searchText = "";
                    toolbarSelectIndex = selectedIndex;
                    isChanged = true;
                    scrollPos = Vector2.zero;
                }
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("GC", EditorStyles.toolbarButton, GUILayout.Width(40)))
                {
                    bundleLoader.UnloadUnusedAssets(null);
                }

                if (GUILayout.Button("Export", EditorStyles.toolbarButton, GUILayout.Width(60)))
                {
                    ExportToDisk(false);
                }
                if (GUILayout.Button("Export All",EditorStyles.toolbarButton,GUILayout.Width(60)))
                {
                    ExportToDisk(true);
                }

                string tempSearchText = searchField.OnToolbarGUI(searchText);
                if(tempSearchText!=searchText)
                {
                    isChanged = true;
                    searchText = tempSearchText;
                }
            }
            EditorGUILayout.EndHorizontal();
            if (isChanged)
            {
                if(toolbarSelectIndex == 0)
                {
                    assetNodes.Clear();
                    foreach(var nodeKVP in assetNodeDic)
                    {
                        AssetNode node = nodeKVP.Value;
                        if(!isShowLoadingAssetNode)
                        {
                            bool isDone = node.AsDynamic().IsDone;
                            if(!isDone)
                            {
                                continue;
                            }
                        }
                        if(string.IsNullOrEmpty(searchText) || nodeKVP.Key.ToLower().Contains(searchText.ToLower()))
                        {
                            assetNodes.Add(node);
                        }
                    }
                    assetNodes.Sort((item1, item2) =>
                    {
                        string path1 = item1.AsDynamic().assetPath;
                        string path2 = item2.AsDynamic().assetPath;
                        return path1.CompareTo(path2);
                    });
                }
                else if(toolbarSelectIndex == 1)
                {
                    bundleNodes.Clear();
                    foreach (var nodeKVP in bundleNodeDic)
                    {
                        BundleNode node = nodeKVP.Value;
                        if (!isShowLoadingBundleNode)
                        {
                            bool isDone = node.AsDynamic().IsDone;
                            if (!isDone)
                            {
                                continue;
                            }
                        }
                        if (string.IsNullOrEmpty(searchText) || nodeKVP.Key.ToLower().Contains(searchText.ToLower()))
                        {
                            bundleNodes.Add(node);
                        }
                    }
                    bundleNodes.Sort((item1, item2) =>
                    {
                        string path1 = item1.AsDynamic().bundlePath;
                        string path2 = item2.AsDynamic().bundlePath;
                        return path1.CompareTo(path2);
                    });
                }
            }

            if(toolbarSelectIndex == 0)
            {
                bool isShowLoading = EditorGUILayout.Toggle("Show Loading Node:", isShowLoadingAssetNode);
                if(isShowLoading!= isShowLoadingAssetNode)
                {
                    isChanged = true;
                    isShowLoadingAssetNode = isShowLoading;
                }
                isShowAssetNodeMainBundle = EditorGUILayout.Toggle("Show Main Bundle:", isShowAssetNodeMainBundle);
                isShowAssetNodeDependBundle = EditorGUILayout.Toggle("Show Depend Bundle:", isShowAssetNodeDependBundle);
            }
            else if(toolbarSelectIndex == 1)
            {
                bool isShowLoading = EditorGUILayout.Toggle("Show Loading Node:", isShowLoadingBundleNode);
                if (isShowLoading != isShowLoadingBundleNode)
                {
                    isChanged = true;
                    isShowLoadingBundleNode = isShowLoading;
                }
                isShowBundleNodeDirect = EditorGUILayout.Toggle("Show Depend Bundle:", isShowBundleNodeDirect);
            }

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos,EditorStyles.helpBox);
            {
                if(toolbarSelectIndex == 0)
                {
                    DrawAssetNodes();
                }else if(toolbarSelectIndex == 1)
                {
                    DrawBundleNodes();
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private bool InitLoader()
        {
            if (!EditorApplication.isPlaying)
            {
                return false;
            }
            if (bundleLoader != null)
            {
                return true;
            }

            bool isInitSuccess = (bool)AssetManager.GetInstance().AsDynamic().isInit;
            if (!isInitSuccess)
            {
                return false;
            }

            AssetManager assetManager = AssetManager.GetInstance();
            dynamic loader = (AssetBundleLoader)assetManager.AsDynamic().assetLoader;
            if (!(loader is AssetBundleLoader))
            {
                return false;
            }
            bundleLoader = loader;
            dynamic loaderDynamic = bundleLoader.AsDynamic();
            assetAddressConfig = loaderDynamic.assetAddressConfig;
            assetBundleManifest = loaderDynamic.assetBundleManifest;
            assetNodeDic = loaderDynamic.assetNodeDic;
            bundleNodeDic = loaderDynamic.bundleNodeDic;
            return true;
        }
        
        private void DrawAssetNodes()
        {
            foreach (var node in assetNodes)
            {
                string assetPath = node.AsDynamic().assetPath;
                if (!assetNodeFoldoutDic.TryGetValue(assetPath, out bool isFoldout))
                {
                    isFoldout = false;
                    assetNodeFoldoutDic.Add(assetPath, isFoldout);
                }

                bool isDone = node.AsDynamic().IsDone;
                if(!isDone)
                {
                    assetPath += "(Loading)";
                }

                assetNodeFoldoutDic[assetPath] = EditorGUILayout.Foldout(isFoldout, assetPath);
                if (isFoldout)
                {
                    DrawAssetNode(node, isShowAssetNodeMainBundle, isShowAssetNodeDependBundle);
                }
            }
        }
        private void DrawAssetNode(AssetNode assetNode,bool showMainBundle = false,bool showDependBundle = false)
        {
            dynamic assetNodeDynamic = assetNode.AsDynamic();
            string assetPath = assetNodeDynamic.assetPath;
            bool isDone = assetNodeDynamic.IsDone;
            bool isAlive = assetNodeDynamic.IsAlive();
            int loadCount = assetNodeDynamic.loadCount;
            List<WeakReference> weakAssets = assetNodeDynamic.weakAssets;

            EditorGUIUtil.BeginGUIBackgroundColor(nodeBGColor);
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUIUtil.BeginGUIColor(Color.white);
                    {
                        EditorGUIUtil.BeginIndent();
                        {
                            EditorGUILayout.LabelField("Asset Node:");
                            EditorGUIUtil.BeginIndent();
                            {
                                EditorGUILayout.LabelField($"Asset Path:{assetPath}{(isDone?"":"  (Loading)")}");
                                EditorGUILayout.LabelField($"Is Alive:{isAlive}");
                                EditorGUILayout.LabelField($"Load Count:{loadCount}");
                                if (weakAssets.Count > 0)
                                {
                                    EditorGUILayout.LabelField("Weak Ref:");
                                    EditorGUIUtil.BeginIndent();
                                    {
                                        foreach (var weakRef in weakAssets)
                                        {
                                            if (!UnityObjectExtension.IsNull(weakRef.Target))
                                            {
                                                UnityObject uObj = weakRef.Target as UnityObject;
                                                EditorGUILayout.LabelField($"Name:{uObj.name}");
                                            }
                                        }
                                    }
                                    EditorGUIUtil.EndIndent();
                                }
                            }
                            EditorGUIUtil.EndIndent();

                            if (showMainBundle || showDependBundle)
                            {
                                BundleNode mainBundleNode = assetNodeDynamic.bundleNode;
                                string mainBundlePath = mainBundleNode.AsDynamic().bundlePath;
                                EditorGUILayout.LabelField("Bundle Node:");
                                EditorGUIUtil.BeginIndent();
                                {
                                    if(showMainBundle)
                                    {
                                        EditorGUILayout.LabelField("Main Bundle:");
                                        EditorGUIUtil.BeginIndent();
                                        {
                                            DrawBundleNode(mainBundleNode);
                                        }
                                        EditorGUIUtil.EndIndent();
                                    }
                                    if(showDependBundle)
                                    {
                                        EditorGUILayout.LabelField("Depend Bundle:");
                                        string[] depends = assetBundleManifest.GetAllDependencies(mainBundlePath);
                                        EditorGUIUtil.BeginIndent();
                                        {
                                            foreach(var depend in depends)
                                            {
                                                BundleNode dependNode = bundleNodeDic[depend];
                                                DrawBundleNode(dependNode);
                                                EditorGUILayout.Space();
                                            }
                                        }
                                        EditorGUIUtil.EndIndent();
                                    }
                                }
                                EditorGUIUtil.EndIndent();

                            }
                        }
                        EditorGUIUtil.EndIndent();
                    }
                    EditorGUIUtil.EndGUIColor();

                }
                EditorGUILayout.EndVertical();
            }
            EditorGUIUtil.EndGUIBackgroundColor();
        }

        private void ExportAssetNodes(string fileDiskPath,bool isAll)
        {
            StringBuilder sb = new StringBuilder();
            List<AssetNode> exportNodes = new List<AssetNode>();
            if(isAll)
            {
                exportNodes.AddRange(assetNodeDic.Values.ToArray());
            }else
            {
                exportNodes.AddRange(assetNodes);
            }
            foreach(var node in exportNodes)
            {
                dynamic nodeDynamic = node.AsDynamic();
                string assetPath = nodeDynamic.assetPath;
                BundleNode mainBundleNode = nodeDynamic.bundleNode;
                int loadCount = nodeDynamic.loadCount;

                sb.AppendLine($"Asset Path:{assetPath}");
                sb.AppendLine($"Load Count:{loadCount}");
                if(isShowAssetNodeMainBundle)
                {
                    sb.AppendLine("Main Bundle:");
                    sb.AppendLine(GetBundleNodeDesc(mainBundleNode, "    "));
                }
                if(isShowAssetNodeDependBundle)
                {
                    string mainBundlePath = mainBundleNode.AsDynamic().bundlePath;
                    string[] depends = assetBundleManifest.GetAllDependencies(mainBundlePath);
                    sb.AppendLine("Depend Bundle:");
                    foreach (var depend in depends)
                    {
                        BundleNode dependNode = bundleNodeDic[depend];
                        sb.AppendLine(GetBundleNodeDesc(dependNode, "    "));
                    }
                }
                sb.AppendLine();
            }
            File.WriteAllText(fileDiskPath, sb.ToString());
        }

        private void DrawBundleNodes()
        {
            foreach (var node in bundleNodes)
            {
                string bundlePath = node.AsDynamic().bundlePath;
                if (!bundleNodeFoldoutDic.TryGetValue(bundlePath, out bool isFoldout))
                {
                    isFoldout = false;
                    bundleNodeFoldoutDic.Add(bundlePath, isFoldout);
                }

                bool isDone = node.AsDynamic().IsDone;
                if(!isDone)
                {
                    bundlePath += "(Loading)";
                }

                bundleNodeFoldoutDic[bundlePath] = EditorGUILayout.Foldout(isFoldout, bundlePath);
                if (isFoldout)
                {
                    EditorGUIUtil.BeginGUIBackgroundColor(nodeBGColor);
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        {
                            EditorGUIUtil.BeginIndent();
                            {
                                EditorGUILayout.LabelField("Bundle Node:");
                                EditorGUIUtil.BeginIndent();
                                {
                                    DrawBundleNode(node);
                                }
                                EditorGUIUtil.EndIndent();

                                if (isShowBundleNodeDirect)
                                {
                                    EditorGUILayout.LabelField("Direct Bundle:");
                                    string[] depends = assetBundleManifest.GetDirectDependencies(bundlePath);
                                    EditorGUIUtil.BeginIndent();
                                    {
                                        foreach (var depend in depends)
                                        {
                                            BundleNode dependNode = bundleNodeDic[depend];
                                            DrawBundleNode(dependNode);
                                            EditorGUILayout.Space();
                                        }
                                    }
                                    EditorGUIUtil.EndIndent();
                                }
                            }
                            EditorGUIUtil.EndIndent();
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUIUtil.EndGUIBackgroundColor();
                }
            }
        }
        private void DrawBundleNode(BundleNode bundleNode)
        {
            dynamic bundleNodeDynamic = bundleNode.AsDynamic();
            string bundlePath = bundleNodeDynamic.bundlePath;
            bool isDone = bundleNodeDynamic.IsDone;
            int refCount = bundleNodeDynamic.RefCount;
            
            EditorGUILayout.LabelField($"Bundle Path:{bundlePath}{(isDone?"":"   (Loading)")}");
            if(isDone)
            {
                bool isScene = bundleNodeDynamic.IsScene;
                EditorGUILayout.LabelField($"IsScene:{isScene}");
            }
            EditorGUILayout.LabelField($"Ref Count:{refCount}");
        }

        private void ExportBundleNodes(string fileDiskPath, bool isAll)
        {
            StringBuilder sb = new StringBuilder();
            List<BundleNode> exportNodes = new List<BundleNode>();
            if (isAll)
            {
                exportNodes.AddRange(bundleNodeDic.Values.ToArray());
            }
            else
            {
                exportNodes.AddRange(bundleNodes);
            }
            foreach (var node in exportNodes)
            {
                sb.AppendLine(GetBundleNodeDesc(node));
                if(isShowBundleNodeDirect)
                {
                    string bundlePath = node.AsDynamic().bundlePath;
                    string[] depends = assetBundleManifest.GetAllDependencies(bundlePath);
                    sb.AppendLine("Depend Bundle:");
                    foreach (var depend in depends)
                    {
                        sb.AppendLine("    " + depend);
                    }
                }
                sb.AppendLine();
            }
            File.WriteAllText(fileDiskPath, sb.ToString());
        }

        private string GetBundleNodeDesc(BundleNode bundleNode,string prefix = "")
        {
            dynamic bundleNodeDynamic = bundleNode.AsDynamic();
            string bundlePath = bundleNodeDynamic.bundlePath;
            bool isScene = bundleNodeDynamic.IsScene();
            int refCount = bundleNodeDynamic.RefCount;

            return $"{prefix}Bundle Path:{bundlePath}\n{prefix}Ref Count:{refCount}\n{prefix}IsScene:{isScene}";
        }

        private void ExportToDisk(bool isAll)
        {
            string defaultFileName = "";
            if (toolbarSelectIndex == 0)
            {
                defaultFileName = "asset_nodes";
            }
            else if (toolbarSelectIndex == 1)
            {
                defaultFileName = "bundle_nodes";
            };

            string fileDiskPath = EditorUtility.SaveFilePanel("Save Nodes", "D:\\", defaultFileName, "txt");
            if (!string.IsNullOrEmpty(fileDiskPath))
            {
                if (toolbarSelectIndex == 0)
                {
                    ExportAssetNodes(fileDiskPath, isAll);
                }
                else if (toolbarSelectIndex == 1)
                {
                    ExportBundleNodes(fileDiskPath, isAll);
                }
            }
        }
    }
}
