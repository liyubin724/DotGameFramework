using Dot.Core.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Dot.Tests
{
    public class TestAssetManager : MonoBehaviour
    {
        
        private void Awake()
        {
            GameController.StartUp();

            //string bundlePath = "D:/assetbundles/StandaloneWindows64/assetbundles/assets/resources/prefabs/capsule_prefab";
            //AssetBundle ab = AssetBundle.LoadFromFile(bundlePath);
            //GameObject prefab = ab.LoadAsset<GameObject>("Assets/Resources/Prefabs/Capsule.prefab");
            //GameObject.Instantiate(prefab);


            AssetManager.GetInstance().InitLoader(AssetLoaderMode.AssetBundle, AssetPathMode.Path, (isSuccess) =>
            {
                Debug.Log("Asset Manager init Success");
            }, "D:/assetbundles/StandaloneWindows64/assetbundles",5.0f);
        }
        private string[] assetAssetPathArr = new string[]
        {
            "Assets/Resources/Prefabs/AtlasImage.prefab",
            "Assets/Resources/Prefabs/Capsule.prefab",
            "Assets/Resources/Prefabs/Cylinder.prefab",
            "Assets/Resources/Prefabs/Sphere.prefab",
        };
        private void OnGUI()
        {
            if (GUILayout.Button("Unload Unused Asset"))
            {
                AssetManager.GetInstance().UnloadUnusedAsset(null);
            }
            if (GUILayout.Button("Resources Load Prefab"))
            {
                AssetManager.GetInstance().LoadAssetAsync(assetAssetPathArr[0], (assetPath, uObj, userData) =>
                 {
                     AssetManager.GetInstance().InstantiateAsset(assetAssetPathArr[0], uObj);
                     //GameObject.Instantiate<GameObject>(uObj as GameObject);
                 });
            }

            if (GUILayout.Button("Resources Load Prefab"))
            {
                AssetManager.GetInstance().LoadBatchAssetAsync(assetAssetPathArr, (assetPath, uObj, userData) =>
                {
                    GameObject.Instantiate<GameObject>(uObj as GameObject);
                }, (assetPaths, uObjs, userData) =>
                 {

                 }) ;
            }
            if (GUILayout.Button("Resources Instance Prefab"))
            {
                AssetManager.GetInstance().InstanceAssetAsync(assetAssetPathArr[0], (assetPath, uObj, userData) =>
                {
                });
            }
            if (GUILayout.Button("Resources Load Prefab"))
            {
                AssetManager.GetInstance().InstanceBatchAssetAsync(assetAssetPathArr, (assetPath, uObj, userData) =>
                {
                }, (assetPaths, uObjs, userData) =>
                {

                });
            }
        }
    }
}
