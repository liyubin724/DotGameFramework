using Dot.Core.Loader;
using System.Collections.Generic;
using UnityEngine;
using SystemObject = System.Object;
using UnityObject = UnityEngine.Object;

namespace Dot.Tests
{
    public enum AssetManagerInitStatus
    {
        None,
        Initing,
        Inited,
        InitError,
    }
    public class TestAssetManager : MonoBehaviour
    {
        public class TestData
        {
            public string address;
            public string assetPath;
        }
        private List<TestData> datas = new List<TestData>();
        public Canvas rootCanvas = null;
        private void Awake()
        {
            GameController.StartUp();
            datas.Add(new TestData() { address = "AtlasImage", assetPath = "Assets/ArtRes/Prefabs/AtlasImage.prefab" });
            datas.Add(new TestData() { address = "Capsule", assetPath = "Assets/ArtRes/Prefabs/Capsule.prefab" });
            datas.Add(new TestData() { address = "Cylinder", assetPath = "Assets/ArtRes/Prefabs/Cylinder.prefab" });
            datas.Add(new TestData() { address = "Sphere", assetPath = "Assets/ArtRes/Prefabs/Sphere.prefab" });
        }
        private AssetManagerInitStatus initStatus = AssetManagerInitStatus.None;
        private AssetPathMode assetPathMode = AssetPathMode.Address;
        private AssetLoaderMode assetLoaderMode = AssetLoaderMode.AssetDatabase;
        private void OnInitAssetManager()
        {
            GUILayout.BeginArea(new Rect(0, 0, 200, Screen.height));
            {
                if (initStatus == AssetManagerInitStatus.None)
                {
                    GUILayout.Label("Choose Asset Loader Mode:");
                    bool result = GUILayout.Toggle(assetLoaderMode == AssetLoaderMode.AssetDatabase, "AssetDatabaseMode");
                    if (result) assetLoaderMode = AssetLoaderMode.AssetDatabase; else assetLoaderMode = AssetLoaderMode.AssetBundle;
                    result = GUILayout.Toggle(assetLoaderMode == AssetLoaderMode.AssetBundle, "AssetBundleMode");
                    if (result) assetLoaderMode = AssetLoaderMode.AssetBundle;

                    GUILayout.Label("Choose Asset Path Mode:");
                    bool pathModeResult = GUILayout.Toggle(assetPathMode == AssetPathMode.Address, "Address");
                    if (pathModeResult) assetPathMode = AssetPathMode.Address; else assetPathMode = AssetPathMode.Path;
                    pathModeResult = GUILayout.Toggle(assetPathMode == AssetPathMode.Path, "Path");
                    if (pathModeResult) assetPathMode = AssetPathMode.Path;

                    if (GUILayout.Button("Init Asset Manager"))
                    {
                        SystemObject[] datas = null;
                        if (assetLoaderMode == AssetLoaderMode.AssetBundle)
                        {
                            datas = new SystemObject[2];
                            datas[0] = "D:/assetbundles/StandaloneWindows64/assetbundles";
                            datas[1] = 5.0f;
                        }
                        initStatus = AssetManagerInitStatus.Initing;

                        //AssetManager.GetInstance().InitLoader(assetLoaderMode, assetPathMode, (isSuccess) =>
                        //{
                        //    if (isSuccess)
                        //    {
                        //        initStatus = AssetManagerInitStatus.Inited;
                        //    }
                        //    else
                        //    {
                        //        initStatus = AssetManagerInitStatus.InitError;
                        //    }
                        //}, datas);
                    }
                }
                else
                {
                    GUILayout.Label("Asset Loader Mode:" + assetLoaderMode);
                    GUILayout.Label("Asset Path Mode:" + assetPathMode);
                    GUILayout.Space(10);

                    if (initStatus == AssetManagerInitStatus.Initing)
                    {
                        GUILayout.Label("Init ......");
                    }
                    else if (initStatus == AssetManagerInitStatus.Inited)
                    {
                        GUILayout.Label("Init Complete");
                    }
                    else if (initStatus == AssetManagerInitStatus.InitError)
                    {
                        GUILayout.Label("Init Error");
                    }
                }
            }
            GUILayout.EndArea();
        }

        private List<UnityObject> cachedObjects = new List<UnityObject>();

        private void OnAssetOperation()
        {
            GUILayout.BeginArea(new Rect(205, 0, 200, Screen.height));
            {
                if(initStatus == AssetManagerInitStatus.Inited)
                {
                    GUILayout.Label("Cached Objects Count:" + cachedObjects.Count);

                    if (GUILayout.Button("Unload Unused Asset"))
                    {
                        AssetManager.GetInstance().UnloadUnusedAsset(null);
                    }

                    if (GUILayout.Button("Random Load Asset"))
                    {
                        UnityEngine.Random.InitState((int)Time.realtimeSinceStartup);
                        int index = UnityEngine.Random.Range(0, datas.Count);
                        TestData data = datas[index];

                        AssetManager.GetInstance().LoadAssetAsync(assetPathMode == AssetPathMode.Address?data.address:data.assetPath, 
                            (assetPath, uObj, userData) =>{
                                Debug.Log(assetPath);

                                GameObject gObj = AssetManager.GetInstance().InstantiateAsset(assetPath, uObj) as GameObject;
                                if(assetPath.IndexOf("AtlasImage")>=0)
                                {
                                    gObj.transform.SetParent(rootCanvas.transform, false);
                                }
                                cachedObjects.Add(gObj);
                            });
                    }

                    if(GUILayout.Button("Random Load Muli Asset"))
                    {
                        UnityEngine.Random.InitState((int)Time.realtimeSinceStartup);
                        int index = UnityEngine.Random.Range(1, 10);
                        List<string> paths = new List<string>();
                        for(int i =0;i<index;i++)
                        {
                            UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks+i);
                            int di = UnityEngine.Random.Range(0, datas.Count);
                            TestData data = datas[di];
                            paths.Add(assetPathMode == AssetPathMode.Address ? data.address : data.assetPath);
                        }
                        AssetManager.GetInstance().LoadBatchAssetAsync(paths.ToArray(), (assetPath, uObj, userData) =>
                        {
                            Debug.Log(assetPath);

                            GameObject gObj = AssetManager.GetInstance().InstantiateAsset(assetPath, uObj) as GameObject;
                            if (assetPath.IndexOf("AtlasImage") >= 0)
                            {
                                gObj.transform.SetParent(rootCanvas.transform, false);
                            }
                            cachedObjects.Add(gObj);
                        }, (assetPaths, uObjs, userData) =>
                        {

                        });
                    }
                    if (GUILayout.Button("Delete All"))
                    {
                        foreach (var obj in cachedObjects)
                        {
                            UnityObject.Destroy(obj);
                        }
                        cachedObjects.Clear();
                    }
                }
            }
            GUILayout.EndArea();
        }
        
        private void OnGUI()
        {
            OnInitAssetManager();
            OnAssetOperation();
        }
    }
}
