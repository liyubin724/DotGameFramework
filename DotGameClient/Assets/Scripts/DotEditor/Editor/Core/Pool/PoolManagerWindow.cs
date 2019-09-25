using Dot.Core.Pool;
using DotEditor.Core.EGUI;
using ReflectionMagic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace DotEditor.Core.Pool
{
    public class PoolManagerWindow : EditorWindow
    {
        [MenuItem("Game/Pool/Pool Manager Window")]
        public static void ShowWin()
        {
            PoolManagerWindow win = GetWindow<PoolManagerWindow>();
            win.titleContent = new GUIContent("Pool Manager");
            win.autoRepaintOnSceneChange = true;
            win.Show();
        }

        private Dictionary<string, bool> spawnPoolFoldoutDic = new Dictionary<string, bool>();
        private Vector2 scrollPos = Vector2.zero;

        private void OnGUI()
        {
            if (!EditorApplication.isPlaying)
            {
                return;
            }

            PoolManager poolManager = PoolManager.GetInstance();
            var dynamicPoolMgr = poolManager.AsDynamic();
            Dictionary<string, SpawnPool> spawnDic = dynamicPoolMgr.spawnDic;
            List<PoolData> poolDatas = dynamicPoolMgr.poolDatas;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Base Info", EditorGUIStyle.GetBoldLabelStyle(20),GUILayout.Height(25));
            EditorGUILayout.BeginVertical(EditorStyles.helpBox,GUILayout.ExpandWidth(true));
            {
                Transform cachedTransform = dynamicPoolMgr.cachedTransform;
                EditorGUILayout.ObjectField("Root Transform", cachedTransform, typeof(Transform),false);
                float cullTimeInterval = dynamicPoolMgr.cullTimeInterval;
                EditorGUILayout.LabelField("Cull Time Interval", ""+cullTimeInterval);
                EditorGUILayout.LabelField("Loading Count", "" + poolDatas.Count);
            }
            EditorGUILayout.EndVertical();
            
            if(poolDatas.Count>0)
            {
                EditorGUILayout.LabelField("PoolData List", EditorGUIStyle.GetBoldLabelStyle(20));
                EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandWidth(true));
                {
                    foreach(var poolData in poolDatas)
                    {

                    }
                }
                EditorGUILayout.EndVertical();
            }
        }
    }
}
