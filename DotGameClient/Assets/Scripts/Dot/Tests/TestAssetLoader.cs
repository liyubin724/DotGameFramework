using Dot.Core.Asset;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using System;
using UnityEngine.UI;


public class TestAssetLoader : MonoBehaviour
{
    AssetManager loader = new AssetManager();

    public RawImage image1 = null;
    public RawImage image2 = null;
    void Start()
    {

    }
 
    void Update()
    {
        System.GC.Collect();
        loader.DoUpdate();
    }

    private List<GameObject> objs = new List<GameObject>();
    private AssetHandle assetHandle1 = null;
    private AssetHandle assetHandle2 = null;
    private void OnGUI()
    {
        if(GUILayout.Button("Load Asset"))
        {
            assetHandle1 = loader.LoadAssetAsync("mat_texture", (address, uObj) =>
            {
                image1.texture = (Texture)uObj;
            }, null);
            assetHandle2 = loader.LoadAssetAsync("mat_texture", (address, uObj) =>
            {
                image2.texture = (Texture)uObj;
            }, null);
        }

        if(GUILayout.Button("Release Asset"))
        {
            if(image1.texture!=null)
            {
                image1.texture = null;
                assetHandle1.Release();
            }else if(image2.texture!=null)
            {
                image2.texture = null;
                assetHandle2.Release();
            }
        }

        if(GUILayout.Button("Load Prefab"))
        {
            loader.InstanceAssetAsync("prefab", OnPrefabLoadFinish, OnPrefabLoadProgress);
        }

        if(GUILayout.Button("Release Prefab"))
        {
            objs.ForEach((gObj) => GameObject.Destroy(gObj));
            objs.Clear();
        }
    }

    private void OnPrefabLoadFinish(string address,UnityObject uObj)
    {
        (uObj as GameObject).transform.SetParent(transform, false);
        objs.Add(uObj as GameObject);

    }

    private void OnPrefabLoadProgress(string address,float progress)
    {
        Debug.Log("Progress = " + progress);
    }
}
