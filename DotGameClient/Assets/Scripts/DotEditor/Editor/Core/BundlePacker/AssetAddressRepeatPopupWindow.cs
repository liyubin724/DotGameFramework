using Dot.Core.Loader.Config;
using DotEditor.Core.EGUI;
using DotEditor.Core.Window;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace DotEditor.Core.Packer
{
    public class AssetAddressRepeatPopupWindow : DotPopupWindow
    {
        private static int WIN_WIDTH = 600;
        private static int WIN_Height = 300;

        private Vector2 scrollPos = Vector2.zero;
        private List<AssetAddressData> repeatAddressList;

        public static AssetAddressRepeatPopupWindow GetWindow()
        {
            return GetPopupWindow<AssetAddressRepeatPopupWindow>();
        }

        public void ShowWithParam(List<AssetAddressData> list,Vector2 position)
        {
            repeatAddressList = list;
            Show<AssetAddressRepeatPopupWindow>(new Rect(position+new Vector2(10,20), new Vector2(WIN_WIDTH, WIN_Height)), true, true);
        }

        protected override void OnGUI()
        {
            base.OnGUI();
            GUIStyle boldCenterStyle = new GUIStyle(EditorStyles.label);
            boldCenterStyle.alignment = TextAnchor.MiddleCenter;
            boldCenterStyle.fontStyle = FontStyle.Bold;
            EditorGUILayout.LabelField("Repeat Address", boldCenterStyle);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            {
                int index = 0;
                foreach(var data in repeatAddressList)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("" + index, GUILayout.Width(20));
                        EditorGUIUtil.BeginLabelWidth(60);
                        {
                            EditorGUILayout.TextField("assetPath",data.assetPath);
                        }
                        EditorGUIUtil.EndLableWidth();
                        UnityObject uObj = AssetDatabase.LoadAssetAtPath<UnityObject>(data.assetPath);
                        EditorGUILayout.ObjectField(uObj, typeof(UnityObject), true, GUILayout.Width(120));
                    }
                    EditorGUILayout.EndHorizontal();

                    ++index;
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }
}
