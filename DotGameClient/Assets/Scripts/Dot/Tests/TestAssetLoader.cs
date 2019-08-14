using Dot;
using Dot.Core.Asset;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using SystemObject = System.Object;
using UnityObject = UnityEngine.Object;

public class TestAssetLoader : MonoBehaviour
{
    AssetLoader loader = null;

    public RawImage image1 = null;
    public RawImage image2 = null;
    void Start()
    {
        GameController.StartUp();
        loader = AssetLoader.GetInstance();
    }

    private List<GameObject> objs = new List<GameObject>();
    private AssetHandle assetHandle1 = null;
    private AssetHandle assetHandle2 = null;
    private void OnGUI()
    {
        if(GUILayout.Button("Init"))
        {
            Addressables.InitializeAsync().Completed += (handle) =>
            {
                Debug.Log("Init Finished");
                // Addressables.Release(handle);

                Addressables.InstantiateAsync("effect").Completed += (h) =>
                {
                    Addressables.Release(h);
                };
            };
        }

        if(GUILayout.Button("Load Asset"))
        {
            assetHandle1 = loader.LoadAssetAsync("mat_texture", (address, uObj,userData) =>
            {
                image1.texture = (Texture)uObj;
            }, null, null);
            assetHandle2 = loader.LoadAssetAsync("mat_texture", (address, uObj,userData) =>
            {
                image2.texture = (Texture)uObj;
            }, null,null);
        }

        if(GUILayout.Button("Release Asset"))
        {
            if(image1.texture!=null)
            {
                image1.texture = null;
                assetHandle1?.Release();
            }else if(image2.texture!=null)
            {
                image2.texture = null;
                assetHandle2?.Release();
            }
        }

        if(GUILayout.Button("Load Prefab"))
        {
            prefabAssetHandle = loader.InstanceAssetAsync("prefab", OnPrefabLoadFinish, OnPrefabLoadProgress, null);
        }

        if(GUILayout.Button("Release Prefab"))
        {
            objs.ForEach((gObj) => GameObject.Destroy(gObj));
            objs.Clear();
        }

        if(GUILayout.Button("Load prefab By label"))
        {
            loader.InstanceAssetsByLabelAsync("test_prefab", (address, uObj,userData) =>
            {
                Debug.Log("FFFFFFFFFFFF+" + address);
            }, null, (addresses, uObjs,userData) =>
            {
                foreach(var obj in uObjs)
                {
                    (obj as GameObject).transform.localScale = new Vector3(2, 2, 2);
                }
            }, null, null);
        }

        if(GUILayout.Button("Load Image By Labe"))
        {
            loader.LoadAssetsByLabeAsync("test_texture", (address, uObj,userData) =>{
                Debug.Log("FFFFFFFFFFFF+" + address);
            },null,(addresses, uObjs,userData) => {
                image2.texture = (Texture)uObjs[0];
            },null, null);
        }
    }

    private AssetHandle prefabAssetHandle = null;

    private void OnPrefabLoadFinish(string address,UnityObject uObj,SystemObject userData)
    {
        (uObj as GameObject).transform.SetParent(transform, false);
        objs.Add(uObj as GameObject);

    }

    private void OnPrefabLoadProgress(string address,float progress, SystemObject userData)
    {
        Debug.Log("Progress = " + progress);
        prefabAssetHandle.Release();
    }
}
