using DotEditor.Core.Util;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DotEditor.Core.Tools
{
    public static class MissingScriptMenuItem
    {
        [MenuItem("Game/Tools/Game Object/Selected Missing Script")]
        public static void CheckSelectedGameObjectMissingScript()
        {
            GameObject go = Selection.activeGameObject;
            if(go!=null)
            {
                if (UnityObjectUtil.IsMissingScript(go,out string[] paths,false))
                {
                    EditorUtility.DisplayDialog("Failed", "Found Missing Script GameObject.","OK");
                    Debug.LogError("Missing Scripts:\n" + string.Join("\n", paths));
                }else
                {
                    EditorUtility.DisplayDialog("Success", "Not Found.", "OK");
                }
            }
        }

        [MenuItem("Game/Tools/Game Object/All Missing Script")]
        public static void CheckAllGameObjectMissingScript()
        {
            if(!EditorUtility.DisplayDialog("Warning", "All asset in project will be searched ,it maybe cost a lot of time.Are you sure to continue", "OK","Cancel"))
            {
                return;
            }

            string[] assetPaths = AssetDatabaseUtil.FindAssets<GameObject>();
            List<string> missingPathList = new List<string>();
            foreach(var assetPath in assetPaths)
            {
                GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if(UnityObjectUtil.IsMissingScript(gameObject,out string[] missingPaths,false))
                {
                    missingPathList.Add(assetPath);
                    Array.ForEach(missingPaths, (path) =>
                    {
                        missingPathList.Add("    " + path);
                    });
                }
            }

            if(missingPathList.Count>0)
            {
                EditorUtility.DisplayDialog("Failed", "Found Missing Script GameObject.", "OK");
                Debug.LogError("Missing Scripts:\n" + string.Join("\n", missingPathList.ToArray()));
            }
            else
            {
                EditorUtility.DisplayDialog("Success", "Not Found.", "OK");
            }
        }

        [MenuItem("Game/Tools/Scene/Current Scene Missing Script")]
        public static void CheckCurrentSceneMissingScript()
        {
            int sceneCount = SceneManager.sceneCount;
            List<string> missingPathList = new List<string>();
            for (int i = 0; i < sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                if(UnityObjectUtil.IsMissingScript(scene,out string[] missingPaths,false))
                {
                    missingPathList.AddRange(missingPaths);
                }
            }

            if (missingPathList.Count > 0)
            {
                EditorUtility.DisplayDialog("Failed", "Found Missing Script GameObject.", "OK");
                Debug.LogError("Missing Scripts:\n" + string.Join("\n", missingPathList.ToArray()));
            }
            else
            {
                EditorUtility.DisplayDialog("Success", "Not Found.", "OK");
            }
        }

        [MenuItem("Game/Tools/Scene/All Scene Missing Script")]
        public static void CheckAllSceneMissingScript()
        {
            if (!EditorUtility.DisplayDialog("Warning", "All asset in project will be searched ,it maybe cost a lot of time.Are you sure to continue", "OK", "Cancel"))
            {
                return;
            }

            string[] scenePaths = AssetDatabaseUtil.FindScenes();
            List<string> missingPathList = new List<string>();
            foreach (var scenePath in scenePaths)
            {
                EditorSceneManager.OpenScene(scenePath,OpenSceneMode.Single);
                Scene scene = SceneManager.GetActiveScene();

                if (UnityObjectUtil.IsMissingScript(scene, out string[] missingPaths, false))
                {
                    missingPathList.Add(scenePath);
                    Array.ForEach(missingPaths, (path) =>
                    {
                        missingPathList.Add("    " + path);
                    });
                }
            }

            if (missingPathList.Count > 0)
            {
                EditorUtility.DisplayDialog("Failed", "Found Missing Script GameObject.", "OK");
                Debug.LogError("Missing Scripts:\n" + string.Join("\n", missingPathList.ToArray()));
            }
            else
            {
                EditorUtility.DisplayDialog("Success", "Not Found.", "OK");
            }

        }
    }
}
