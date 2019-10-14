using DotEditor.Core.EGUI;
using DotEditor.Core.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace DotEditor.Core.BundleDepend
{
    public class AssetDependDetailPopupWindow : DraggablePopupWindow
    {
        public static void ShowPopupWin(Rect rect,string assetPath,List<AssetData> assets)
        {
            var win = GetPopupWindow<AssetDependDetailPopupWindow>();
            win.assetPath = assetPath;
            win.usedAssetDatas = assets;
            win.Show<AssetDependDetailPopupWindow>(rect, true, false);
        }

        private string assetPath;
        private List<AssetData> usedAssetDatas = null;
        private Vector2 scrollPos = Vector2.zero;

        protected override void OnGUI()
        {
            base.OnGUI();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            {
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField(assetPath);
                        UnityObject uObj = AssetDatabase.LoadAssetAtPath<UnityObject>(assetPath);
                        EditorGUILayout.ObjectField(uObj, typeof(UnityObject), false,GUILayout.MaxWidth(200));
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUIUtil.BeginIndent();
                    {
                        foreach (var data in usedAssetDatas)
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField(data.assetPath);
                                UnityObject uObj = AssetDatabase.LoadAssetAtPath<UnityObject>(data.assetPath);
                                EditorGUILayout.ObjectField(uObj, typeof(UnityObject), false, GUILayout.MaxWidth(200));
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    EditorGUIUtil.EndIndent();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();

            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                if(GUILayout.Button("Close"))
                {
                    Close();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
