using Dot;
using Dot.Core.Loader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAssetBridge : MonoBehaviour
{
    private AssetLoaderBridge loaderBridge = new AssetLoaderBridge(AssetLoaderPriority.Default);

    private void Awake()
    {
        GameController.StartUp();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Init Asset Loader"))
        {
            string assetRoot = "D:/assetbundle/StandaloneWindows64/assetbundles";
            AssetManager.GetInstance().InitLoader(AssetLoaderMode.AssetBundle, AssetPathMode.Address, 1, assetRoot, (isSuccess) =>
            {
                if (isSuccess)
                {
                    Debug.Log("Init Asset Manager Success");
                }
                else
                {
                    Debug.LogError("Init Asset Manager Failed");
                }
            });
        }

        if(GUILayout.Button("Test Bridge"))
        {
            string[] assets = new string[]
            {
                "ch_pc_hou_006.prefab","ch_pc_hou_008.prefab","ch_pc_hou_009.prefab","Sphere.prefab",
            };
            loaderBridge.InstanceBatchAssetAsync(assets, (address, uObj, userData) =>
            {

            }, (addresses, uObjs, userData) =>
            {

            },null);
        }

        if(GUILayout.Button("Stop"))
        {
            loaderBridge.Dispose();
            loaderBridge = new AssetLoaderBridge(AssetLoaderPriority.Default);
        }

    }
}
