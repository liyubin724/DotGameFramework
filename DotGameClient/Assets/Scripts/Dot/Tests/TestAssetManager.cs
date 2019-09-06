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
        public Canvas rootCanvas = null;
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
            "Assets/ArtRes/Prefabs/AtlasImage.prefab",
             "Assets/ArtRes/Prefabs/AtlasImage.prefab",
              "Assets/ArtRes/Prefabs/AtlasImage.prefab",
            //"Assets/ArtRes/Prefabs/Capsule.prefab",
            //"Assets/ArtRes/Prefabs/Cylinder.prefab",
            //"Assets/ArtRes/Prefabs/Sphere.prefab",
        };

        private List<UnityObject> objList = new List<UnityObject>();
        private void OnGUI()
        {
            if (GUILayout.Button("Unload Unused Asset"))
            {
                AssetManager.GetInstance().UnloadUnusedAsset(null);
            }
            if (GUILayout.Button("Load Prefab"))
            {
                AssetManager.GetInstance().LoadAssetAsync(assetAssetPathArr[0], (assetPath, uObj, userData) =>
                 {
                     GameObject gObj = AssetManager.GetInstance().InstantiateAsset(assetPath, uObj) as GameObject;
                     gObj.transform.SetParent(rootCanvas.transform,false);
                     objList.Add(gObj);
                 });
            }

            if(GUILayout.Button("DeleteAll"))
            {
                foreach(var obj in objList)
                {
                    UnityObject.Destroy(obj);

                }
                //objList.Clear();
            }

            if (GUILayout.Button("Load Prefabs"))
            {
                AssetManager.GetInstance().LoadBatchAssetAsync(assetAssetPathArr, (assetPath, uObj, userData) =>
                {
                    GameObject gObj = AssetManager.GetInstance().InstantiateAsset(assetPath, uObj) as GameObject;
                    gObj.transform.SetParent(rootCanvas.transform, false);
                    objList.Add(gObj);
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
