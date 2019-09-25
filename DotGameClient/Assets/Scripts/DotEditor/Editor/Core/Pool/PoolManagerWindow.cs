using Dot.Core.Pool;
using Dot.Core.Util;
using DotEditor.Core.EGUI;
using ReflectionMagic;
using System;
using System.Collections.Generic;
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

        public class SpawnPoolFoldoutData
        {
            public bool isFoldout = false;
            public Dictionary<string, bool> objectPoolFoldout = new Dictionary<string, bool>();
        }

        private Dictionary<string, SpawnPoolFoldoutData> spawnPoolFoldoutDic = new Dictionary<string, SpawnPoolFoldoutData>();
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

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            {
                if (poolDatas.Count > 0)
                {
                    EditorGUILayout.LabelField("PoolData List", EditorGUIStyle.GetBoldLabelStyle(20), GUILayout.Height(25));
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandWidth(true));
                    {
                        for (int i = 0; i < poolDatas.Count; ++i)
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("" + i, EditorGUIStyle.MiddleLeftLabelStyle);
                                DrawPoolData(poolDatas[i]);
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.LabelField("Spawn Pool List", EditorGUIStyle.GetBoldLabelStyle(20), GUILayout.Height(25));
                EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandWidth(true));
                {
                    foreach (var kvp in spawnDic)
                    {
                        DrawSpawnPool(kvp.Value);
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawPoolData(PoolData poolData)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginVertical();
                {
                    string name = poolData.spawnName;
                    EditorGUILayout.TextField("Name", name);
                    string assetPath = poolData.assetPath;
                    EditorGUILayout.TextField("Asset Path", assetPath);
                    bool isAutoClean = poolData.isAutoClean;
                    EditorGUILayout.Toggle("Auto Clean", isAutoClean);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                {
                    string preloadTotalAmount = "" + poolData.preloadTotalAmount;
                    EditorGUILayout.TextField("Preload Total Amount", preloadTotalAmount);
                    string preloadOnceAmount = "" + poolData.preloadOnceAmount;
                    EditorGUILayout.TextField("Preload Once Amount", "" + preloadOnceAmount);
                    OnPoolComplete completeCallback = poolData.completeCallback;
                    EditorGUILayout.TextField("Complete Callback", completeCallback == null ? "Null" : completeCallback.ToString());
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                {
                    bool isCull = poolData.isCull;
                    EditorGUILayout.Toggle("Is Cull", isCull);
                    string cullOnceAmount = "" + poolData.cullOnceAmount;
                    EditorGUILayout.TextField("Cull Once Amount", cullOnceAmount);
                    string cullDelayTime = "" + poolData.cullDelayTime;
                    EditorGUILayout.TextField("Cull Delay Time", cullDelayTime);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                {
                    string limitMaxAmount = "" + poolData.limitMaxAmount;
                    EditorGUILayout.TextField("Limit Max Amount", limitMaxAmount);
                    string limitMinAmount = "" + poolData.limitMinAmount;
                    EditorGUILayout.TextField("Limit Min Amount", limitMinAmount);
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawSpawnPool(SpawnPool spawnPool)
        {
            var dynamicSpawnPool = spawnPool.AsDynamic();
            string poolName = dynamicSpawnPool.PoolName;
            if(!spawnPoolFoldoutDic.TryGetValue(poolName,out SpawnPoolFoldoutData foldoutData))
            {
                foldoutData = new SpawnPoolFoldoutData();
                spawnPoolFoldoutDic.Add(poolName, foldoutData);
            }

            foldoutData.isFoldout = EditorGUILayout.Foldout(foldoutData.isFoldout, poolName);
            if(foldoutData.isFoldout)
            {
                EditorGUIUtil.BeginIndent();
                {
                    Dictionary<string, GameObjectPool> goPools = dynamicSpawnPool.goPools;
                    foreach (var kvp in goPools)
                    {
                        if (!foldoutData.objectPoolFoldout.TryGetValue(kvp.Key, out bool isFoldout))
                        {
                            isFoldout = false;
                            foldoutData.objectPoolFoldout.Add(kvp.Key, isFoldout);
                        }
                        foldoutData.objectPoolFoldout[kvp.Key] = EditorGUILayout.Foldout(foldoutData.objectPoolFoldout[kvp.Key], kvp.Key);
                        if (foldoutData.objectPoolFoldout[kvp.Key])
                        {
                            EditorGUIUtil.BeginIndent();
                            {
                                DrawGameObjectPool(kvp.Value);
                            }
                            EditorGUIUtil.EndIndent();
                        }
                    }
                }
                EditorGUIUtil.EndIndent();
            }
        }

        private void DrawGameObjectPool(GameObjectPool gameObjectPool)
        {
            var dynamicGameObjectPool = gameObjectPool.AsDynamic();

            string assetPath = dynamicGameObjectPool.assetPath;
            EditorGUILayout.TextField("Asset Path", assetPath);

            GameObject templateGO = dynamicGameObjectPool.instanceOrPrefabTemplate;
            EditorGUILayout.ObjectField("Template", templateGO, typeof(GameObject), false);

            bool isInstance = dynamicGameObjectPool.isInstance;
            EditorGUILayout.Toggle("Is Instance", isInstance);

            bool isRuntimeInstance = dynamicGameObjectPool.isRuntimeInstance;
            EditorGUILayout.Toggle("Is Runtime Instance", isRuntimeInstance);

            OnPoolComplete completeCallback = dynamicGameObjectPool.completeCallback;
            EditorGUILayout.TextField("Complete Callback", completeCallback==null?"Null":completeCallback.ToString());

            bool isAutoClean = dynamicGameObjectPool.isAutoClean;
            EditorGUILayout.Toggle("Auto Clean", isAutoClean);
            string preloadTotalAmount = "" + dynamicGameObjectPool.preloadTotalAmount;
            EditorGUILayout.TextField("Preload Total Amount", preloadTotalAmount);
            string preloadOnceAmount = "" + dynamicGameObjectPool.preloadOnceAmount;
            EditorGUILayout.TextField("Preload Once Amount", "" + preloadOnceAmount);

            bool isCull = dynamicGameObjectPool.isCull;
            EditorGUILayout.Toggle("Is Cull", isCull);
            string cullOnceAmount = "" + dynamicGameObjectPool.cullOnceAmount;
            EditorGUILayout.TextField("Cull Once Amount", cullOnceAmount);
            string cullDelayTime = "" + dynamicGameObjectPool.cullDelayTime;
            EditorGUILayout.TextField("Cull Delay Time", cullDelayTime);

            string limitMaxAmount = "" + dynamicGameObjectPool.limitMaxAmount;
            EditorGUILayout.TextField("Limit Max Amount", limitMaxAmount);
            string limitMinAmount = "" + dynamicGameObjectPool.limitMinAmount;
            EditorGUILayout.TextField("Limit Min Amount", limitMinAmount);

            Queue<GameObject> unusedItemQueue = dynamicGameObjectPool.unusedItemQueue;
            if(unusedItemQueue.Count>0)
            {
                EditorGUILayout.LabelField("Unused Items:");
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUIUtil.BeginIndent();
                    {
                        EditorGUIUtil.BeginLabelWidth(100);
                        {
                            int index = 0;
                            foreach (var gObj in unusedItemQueue)
                            {
                                EditorGUILayout.ObjectField("" + index, gObj, typeof(GameObject), false);
                                index++;
                            }
                        }
                        EditorGUIUtil.EndLableWidth();

                    }
                    EditorGUIUtil.EndIndent();
                }
                EditorGUILayout.EndVertical();
            }
            
            List<WeakReference<GameObject>> usedItemList = dynamicGameObjectPool.usedItemList;
            if(usedItemList.Count>0)
            {
                EditorGUILayout.LabelField("Used Items:");
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUIUtil.BeginIndent();
                    {
                        EditorGUIUtil.BeginLabelWidth(40);
                        {
                            int index = 0;
                            foreach (var weakRef in usedItemList)
                            {
                                if (weakRef.TryGetTarget(out GameObject gObj))
                                {
                                    if (!UnityObjectExtension.IsNull(gObj))
                                    {
                                        EditorGUILayout.ObjectField("" + index, gObj, typeof(GameObject), false);
                                        index++;
                                        continue;
                                    }
                                }
                                EditorGUILayout.TextField("" + index, "it has been destroy");

                                index++;
                            }
                        }
                        EditorGUIUtil.EndLableWidth();

                    }
                    EditorGUIUtil.EndIndent();
                }
                EditorGUILayout.EndVertical();
            }
        }
    }
}
