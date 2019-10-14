using DotEditor.Core.EGUI;
using DotEditor.Core.EGUI.TreeGUI;
using DotEditor.Core.Packer;
using DotEditor.Core.Util;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.U2D;

namespace DotEditor.Core.BundleDepend
{
    public class AssetData
    {
        public string assetPath;
        public bool isBundle = false;
        public bool isRepeat = false;

        public List<AssetData> dependAssets = new List<AssetData>();

        public static AssetData Root
        {
            get { return new AssetData(); }
        }

        public bool IsAssetRepeat()
        {
            if (isRepeat) return true;

            List<string> checkDependAssetPaths = new List<string>();

            List<AssetData> tempList = new List<AssetData>();
            tempList.AddRange(dependAssets);
            while(tempList.Count>0)
            {
                AssetData tData = tempList[0];
                tempList.RemoveAt(0);
                if(tData.isBundle)
                {
                    continue;
                }
                if(checkDependAssetPaths.Contains(tData.assetPath))
                {
                    continue;
                }else
                {
                    checkDependAssetPaths.Add(tData.assetPath);
                }
                if(tData.isRepeat)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Contains(string path)
        {
            if (assetPath == path) return true;
            foreach(var data in dependAssets)
            {
                if(data.assetPath == path)
                {
                    return true;
                }
            }
            return false;
        }

        public string ToString(int indent)
        {
            string indentStr = "";
            for(int i =0;i<indent;i++)
            {
                indentStr += "    ";
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{indentStr}assetPath = {assetPath}");
            sb.AppendLine($"{indentStr}isBundle = {isBundle}");
            sb.AppendLine($"{indentStr}isRepeat = {isRepeat}");

            return sb.ToString();
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

        private GUIStyle titleStyle = null;
        private string assetInDiffAtlasStr = "Some asset was found in multiple atlas!!!!\nPlease fixed the issue at first";

        private List<string> packedAssetPaths = new List<string>();
        private Dictionary<string, AssetData> allAssetDic = new Dictionary<string, AssetData>();
        private Dictionary<string, string> assetInAtlasDic = new Dictionary<string, string>();
        private Dictionary<string, List<string>> assetInDiffAtlasDic = new Dictionary<string, List<string>>();

        private void OnEnable()
        {
            RefreshData();
        }

        private void RefreshData()
        {
            AssetBundleTagConfig tagConfig = Util.FileUtil.ReadFromBinary<AssetBundleTagConfig>(BundlePackUtil.GetTagConfigPath());

            packedAssetPaths = (from groupData in tagConfig.groupDatas
                                from detailData in groupData.assetDatas
                                select detailData.assetPath).ToList();

            allAssetDic.Clear();
            assetInAtlasDic.Clear();
            assetInDiffAtlasDic.Clear();

            FindAtlasDepend();
            FindAssetInDiffAtlasData();
            if(assetInDiffAtlasDic.Count == 0)
            {
                FindAssetWithoutAtlasDepend();
                RefreshDependTreeView();
            }
        }

        private void FindAtlasDepend()
        {
            List<string> atlasPaths = (from path in packedAssetPaths
                                       where Path.GetExtension(path).ToLower() == ".spriteatlas"
                                       select path).ToList();

            foreach (var atlasPath in atlasPaths)
            {
                SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);
                if (atlas == null)
                {
                    Debug.LogError("AssetDependWindow::FindAssetDepend->atlas is null.path = " + atlasPath);
                    continue;
                }

                AssetData atlasAssetData = new AssetData();
                atlasAssetData.assetPath = atlasPath;
                atlasAssetData.isBundle = true;
                allAssetDic.Add(atlasPath, atlasAssetData);

                string[] assetInAtlasPaths = SpriteAtlasUtil.GetDependAssets(atlas);
                foreach (var path in assetInAtlasPaths)
                {
                    AssetData spriteAssetData = new AssetData();
                    spriteAssetData.assetPath = path;
                    spriteAssetData.isBundle = true;

                    atlasAssetData.dependAssets.Add(spriteAssetData);

                    if(!assetInAtlasDic.ContainsKey(path))
                    {
                        assetInAtlasDic.Add(path, atlasPath);
                    }else
                    {
                        if(!assetInDiffAtlasDic.TryGetValue(path,out List<string> tList))
                        {
                            tList = new List<string>();
                            assetInDiffAtlasDic.Add(path, tList);
                        }

                        tList.Add(assetInAtlasDic[path]);
                        tList.Add(atlasPath);

                        
                        tList = tList.Distinct().ToList();
                    }
                }
            }
        }

        private void FindAssetInDiffAtlasData()
        {
            string[] validPaths = (from kvp in assetInDiffAtlasDic where kvp.Value.Count <= 1 select kvp.Key).ToArray();
            foreach(var path in validPaths)
            {
                assetInDiffAtlasDic.Remove(path);
            }
        }

        private void FindAssetWithoutAtlasDepend()
        {
            List<string> assetPaths = (from path in packedAssetPaths
                                       where Path.GetExtension(path).ToLower() != ".spriteatlas"
                                       select path).ToList();

            foreach (var path in assetPaths)
            {
                if (!allAssetDic.TryGetValue(path, out AssetData assetData))
                {
                    assetData = new AssetData();
                    assetData.assetPath = path;
                    assetData.isBundle = true;

                    allAssetDic.Add(assetData.assetPath, assetData);
                }
                else
                {
                    continue;
                }

                string[] paths = AssetDatabaseUtil.GetDirectlyDependencies(path, new string[] { ".cs" });
                foreach (var p in paths)
                {
                    FindAssetDirectDepend(p, assetData);
                }
            }

        }

        private void FindAssetDirectDepend(string assetPath, AssetData pAssetData)
        {
            if (assetInAtlasDic.TryGetValue(assetPath, out string atlasPath))
            {
                AssetData atlasAssetData = allAssetDic[atlasPath];
                if(!pAssetData.Contains(assetPath))
                {
                    pAssetData.dependAssets.Add(atlasAssetData);
                }
                return;
            }

            if (allAssetDic.TryGetValue(assetPath, out AssetData assetData))
            {
                if (!pAssetData.Contains(assetPath))
                {
                    pAssetData.dependAssets.Add(assetData);
                }

                if (!assetData.isBundle)
                {
                    assetData.isRepeat = true;
                }

                return;
            }

            assetData = new AssetData();
            assetData.assetPath = assetPath;
            assetData.isBundle = packedAssetPaths.IndexOf(assetPath) >= 0;
            pAssetData.dependAssets.Add(assetData);

            allAssetDic.Add(assetPath, assetData);

            string[] paths = AssetDatabaseUtil.GetDirectlyDependencies(assetPath, new string[] { ".cs" });
            foreach (var path in paths)
            {
                FindAssetDirectDepend(path, assetData);
            }
        }

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

            if (assetInDiffAtlasDic.Count > 0)
            {
                EditorGUILayout.LabelField(new GUIContent("Atlas Error"), titleStyle, GUILayout.Height(24));
                EditorGUILayout.HelpBox(assetInDiffAtlasStr, MessageType.Error);

                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                {
                    EditorGUILayout.BeginVertical();
                    {
                        foreach (var kvp in assetInDiffAtlasDic)
                        {
                            EditorGUILayout.LabelField(kvp.Key);
                            foreach (var p in kvp.Value)
                            {
                                EditorGUIUtil.BeginIndent();
                                {
                                    EditorGUILayout.TextField(p);
                                }
                                EditorGUIUtil.EndIndent();
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndScrollView();
            } else
            {
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
        }

        private void InitDependTreeView()
        {
            dependTreeViewState = new TreeViewState();
            TreeModel<TreeElementWithData<AssetData>> data = new TreeModel<TreeElementWithData<AssetData>>(
               new List<TreeElementWithData<AssetData>>()
               {
                    new TreeElementWithData<AssetData>(AssetData.Root,"",-1,-1),
               });

            dependTreeView = new AssetDependTreeView(dependTreeViewState,data);
        }

        private void RefreshDependTreeView()
        {
            TreeModel<TreeElementWithData<AssetData>> treeModel = dependTreeView.treeModel;
            TreeElementWithData<AssetData> treeModelRoot = treeModel.root;
            treeModelRoot.children?.Clear();

            for (int i = 0; i < packedAssetPaths.Count; ++i)
            {
                string assetPath = packedAssetPaths[i];
                AssetData assetData = allAssetDic[assetPath];

                TreeElementWithData<AssetData> assetPathTreeData = new TreeElementWithData<AssetData>(assetData, "", 0, dependTreeView.NextID);
                treeModel.AddElement(assetPathTreeData, treeModelRoot, treeModelRoot.hasChildren ? treeModelRoot.children.Count : 0);

                if (assetData.dependAssets.Count > 0)
                {
                    TreeElementWithData<AssetData> dependTreeData = new TreeElementWithData<AssetData>(null, "", 1, dependTreeView.NextID);
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
                    EditorApplication.delayCall += RefreshData;
                }

                if (GUILayout.Button("Export All", "toolbarbutton", GUILayout.Width(100)))
                {
                }
                if (GUILayout.Button("Test", "toolbarbutton", GUILayout.Width(100)))
                {
                    //SpriteAtlas atlas = Selection.activeObject as SpriteAtlas;
                    //string[] spritePaths = SpriteAtlasUtil.GetDependAssets(atlas);
                    //foreach (var spritePath in spritePaths)
                    //{
                    //    Debug.Log(spritePath);
                    //}

                    UnityEngine.Object uObj = Selection.activeObject;
                    string ap = AssetDatabase.GetAssetPath(uObj);

                    string[] depends = AssetDatabaseUtil.GetDirectlyDependencies(ap, new string[] { ".cs" });
                    foreach(var d in depends)
                    {
                        Debug.Log(d);
                    }
                }

                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
        }
        
        private void ExportLog()
        {
            //string savedDirPath = EditorUtility.SaveFilePanel("Save Log To", "D:/", "asset_depend_log", ".txt");
            //if(string.IsNullOrEmpty(savedDirPath))
            //{
            //    return;
            //}

            //StringBuilder sb = new StringBuilder();
            //foreach (var path in packedAssetPaths)
            //{
            //    List<string> assetPaths = packedAssetDependDic[path];
            //    sb.AppendLine(path);

            //    foreach (var data in assetPaths)
            //    {
            //        AssetData assetData = allAssetDic[data];

            //        sb.AppendLine(assetData.ToString(1));
            //        sb.AppendLine();
            //    }
            //    sb.AppendLine("---------------------------");
            //}

            //File.WriteAllText(savedDirPath, sb.ToString());
        }
        




    }
}
