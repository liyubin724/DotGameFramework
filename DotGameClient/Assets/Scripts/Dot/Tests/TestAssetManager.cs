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

            AssetManager.GetInstance().InitLoader(AssetLoaderMode.Resources, (isSuccess) =>
            {
                Debug.Log("Asset Manager init Success");
            });
        }
        private string[] resAssetPathArr = new string[]
        {
            "Prefabs/Cube",
            "Prefabs/Capsule",
            "Prefabs/Cylinder",
            "Prefabs/Sphere",
        };
        private void OnGUI()
        {
            if(GUILayout.Button("Resources Load Prefab"))
            {
                AssetManager.GetInstance().LoadAssetAsync(resAssetPathArr[0], (assetPath, uObj, userData) =>
                 {
                     GameObject.Instantiate<GameObject>(uObj as GameObject);
                 });
            }

            if (GUILayout.Button("Resources Load Prefab"))
            {
                AssetManager.GetInstance().LoadBatchAssetAsync(resAssetPathArr, (assetPath, uObj, userData) =>
                {
                    GameObject.Instantiate<GameObject>(uObj as GameObject);
                }, (assetPaths, uObjs, userData) =>
                 {

                 }) ;
            }
            if (GUILayout.Button("Resources Instance Prefab"))
            {
                AssetManager.GetInstance().InstanceAssetAsync(resAssetPathArr[0], (assetPath, uObj, userData) =>
                {
                });
            }
            if (GUILayout.Button("Resources Load Prefab"))
            {
                AssetManager.GetInstance().InstanceBatchAssetAsync(resAssetPathArr, (assetPath, uObj, userData) =>
                {
                }, (assetPaths, uObjs, userData) =>
                {

                });
            }
        }
    }
}
