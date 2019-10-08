using Dot.Core.Loader;
using Dot.Core.Pool;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            datas.Add(new TestData() { address = "ch_pc_hou_006.prefab", assetPath = "Assets/ArtRes/Character/Player/Test01/Prefab/ch_pc_hou_006.prefab" });
            datas.Add(new TestData() { address = "ch_pc_hou_008.prefab", assetPath = "Assets/ArtRes/Character/Player/Test01/Prefab/ch_pc_hou_008.prefab" });
            datas.Add(new TestData() { address = "ch_pc_hou_009.prefab", assetPath = "Assets/ArtRes/Character/Player/Test01/Prefab/ch_pc_hou_009.prefab" });
            datas.Add(new TestData() { address = "Cube.prefab", assetPath = "Assets/ArtRes/Prefabs/Cube.prefab" });
            datas.Add(new TestData() { address = "Sphere.prefab", assetPath = "Assets/ArtRes/Prefabs/Sphere.prefab" });
            datas.Add(new TestData() { address = "testspriteatlas.prefab", assetPath = "Assets/ArtRes/Prefabs/DynamicAtlasImage.prefab" });
            DontDestroyOnLoad(this.gameObject);
        }
        
        private AssetManagerInitStatus initStatus = AssetManagerInitStatus.None;
        private AssetPathMode assetPathMode = AssetPathMode.Address;
        private AssetLoaderMode assetLoaderMode = AssetLoaderMode.AssetBundle;

        private AssetLoaderHandle loaderHandle = null;
        private void OnInitAssetManager()
        {
            GUILayout.BeginArea(new Rect(0, 0, 200, Screen.height));
            {
                if (initStatus == AssetManagerInitStatus.None)
                {
                    GUILayout.Label("Choose Asset Loader Mode:");
                    bool result = GUILayout.Toggle(assetLoaderMode == AssetLoaderMode.AssetBundle, "AssetBundleMode");
                    if (result) assetLoaderMode = AssetLoaderMode.AssetBundle; else assetLoaderMode = AssetLoaderMode.AssetDatabase;
                    result = GUILayout.Toggle(assetLoaderMode == AssetLoaderMode.AssetDatabase, "AssetDatabaseMode");
                    if (result) assetLoaderMode = AssetLoaderMode.AssetDatabase;

                    GUILayout.Label("Choose Asset Path Mode:");
                    bool pathModeResult = GUILayout.Toggle(assetPathMode == AssetPathMode.Address, "Address");
                    if (pathModeResult) assetPathMode = AssetPathMode.Address; else assetPathMode = AssetPathMode.Path;
                    pathModeResult = GUILayout.Toggle(assetPathMode == AssetPathMode.Path, "Path");
                    if (pathModeResult) assetPathMode = AssetPathMode.Path;

                    if (GUILayout.Button("Init Asset Manager"))
                    {
                        int maxLoadingCount = 1;
                        string assetRoot = "D:/assetbundle/StandaloneWindows64/assetbundles";
  
                        initStatus = AssetManagerInitStatus.Initing;

                        AssetManager.GetInstance().InitLoader(assetLoaderMode, assetPathMode,maxLoadingCount,assetRoot, (isSuccess) =>
                        {
                            if (isSuccess)
                            {
                                initStatus = AssetManagerInitStatus.Inited;
                            }
                            else
                            {
                                initStatus = AssetManagerInitStatus.InitError;
                            }
                        });
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

        private List<GameObject> cachedObjects = new List<GameObject>();
        int assetTestIndex = 0;
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
                        TestData data = datas[5];//[index];
                        //if(assetTestIndex % 2 == 0)
                        //{
                        //    data = datas[4];
                        //}
                        //assetTestIndex++;

                        loaderHandle = AssetManager.GetInstance().LoadAssetAsync(assetPathMode == AssetPathMode.Address?data.address:data.assetPath, 
                            (assetPath, uObj, userData) =>{
                                Debug.Log(assetPath);

                                GameObject gObj = AssetManager.GetInstance().InstantiateAsset(assetPath, uObj) as GameObject;
                                if(assetPath.IndexOf("AtlasImage")>=0)
                                {
                                    gObj.transform.SetParent(rootCanvas.transform, false);
                                }
                                cachedObjects.Add(gObj);

                                loaderHandle = null;
                            },AssetLoaderPriority.Default,(assetPath,progress,userData)=>
                            {
                                Debug.Log($"Loading Progress:assetPath ={assetPath},progress={progress}");
                            });
                    }

                    if(GUILayout.Button("Random Load Muli Asset"))
                    {
                        UnityEngine.Random.InitState((int)Time.realtimeSinceStartup);
                        int index = UnityEngine.Random.Range(1, 10);
                        List<string> paths = new List<string>();
                        for (int i = 0; i < index; i++)
                        {
                            UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks + i);
                            int di = UnityEngine.Random.Range(0, datas.Count);
                            TestData data = datas[di];
                            paths.Add(assetPathMode == AssetPathMode.Address ? data.address : data.assetPath);
                        }
                        loaderHandle = AssetManager.GetInstance().LoadBatchAssetAsync(paths.ToArray(), (assetPath, uObj, userData) =>
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
                            loaderHandle = null;
                        });
                    }
                    if (GUILayout.Button("Stop Loader "))
                    {
                        if(loaderHandle!=null)
                        {
                            AssetManager.GetInstance().UnloadAssetLoader(loaderHandle);
                            loaderHandle = null;
                        }
                    }

                    if(GUILayout.Button("Pool"))
                    {
                        PoolData poolData = new PoolData()
                        {
                            spawnName = "CubePool",
                            assetPath = "Cube.prefab",
                            preloadTotalAmount = 10,
                            completeCallback = (spawnName, assetPath) =>
                            {
                                Debug.Log("Pool Create Success");
                            },
                        };
                        PoolManager.GetInstance().LoadAssetToCreateGameObjectPool(poolData);
                    }

                    if(GUILayout.Button("Get From Pool"))
                    {
                        GameObjectPool goPool = PoolManager.GetInstance().GetSpawnPool("CubePool").GetGameObjectPool("Cube.prefab");
                        GameObjectPoolItem poolItem = goPool.GetComponentItem<GameObjectPoolItem>(true,true);
                        poolItem.transform.SetParent(null, false);
                        cachedObjects.Add(poolItem.gameObject);
                    }

                    if(GUILayout.Button("Clean Pool"))
                    {
                        PoolManager.GetInstance().DeleteSpawnPool("CubePool");
                    }

                    if(GUILayout.Button("Load Scene"))
                    {
                        string sceneAddress = "other_scene";
                        AssetManager.GetInstance().LoadSceneAsync(sceneAddress, (address, userData) =>
                        {
                            
                        },null, LoadSceneMode.Additive, true,null);
                    }
                    if (GUILayout.Button("Unload Scene"))
                    {
                        string sceneAddress = "other_scene";
                        AssetManager.GetInstance().UnloadSceneAsync(sceneAddress, null,null,null);
                    }

                    if (GUILayout.Button("Delete All"))
                    {
                        foreach (var obj in cachedObjects)
                        {
                            GameObjectPoolItem poolItem = obj.GetComponent<GameObjectPoolItem>();
                            if(poolItem!=null)
                            {
                                poolItem.ReleaseItem();
                            }else
                            {
                                UnityObject.Destroy(obj);
                            }
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
