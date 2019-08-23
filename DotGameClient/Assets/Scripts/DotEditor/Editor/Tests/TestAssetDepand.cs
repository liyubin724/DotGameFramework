using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityObject = UnityEngine.Object;
using DotEditor.Core.Asset;
using DotEditor.Core.Util;

public class TestAssetDepand 
{
    [MenuItem("Test/TestDepend")]
    public static void FindDepand()
    {
        //string[] shaders = AssetDatabase.FindAssets("t:shader");
        //foreach(var shader in shaders)
        //{
        //    string assetPath = AssetDatabase.GUIDToAssetPath(shader);
        //    Debug.Log("Shader = " + assetPath);
        //}

        UnityObject selObj = Selection.activeObject;

        Texture texture = selObj as Texture;
        if(texture!=null)
        {
            Debug.Log(EditorUtility.FormatBytes(AssetDatabaseUtil.GetTextureStorageSize(texture)));
        }
            Debug.Log(EditorUtility.FormatBytes(AssetDatabaseUtil.GetAssetRuntimeMemorySize(selObj)));

        if(selObj != null)
        {
            AssetDependData dependData = new AssetDependData();
            dependData.AddMainAsset("Assets/ArtRes/UI/Atlas/common_atlas.spriteatlas", false);
            dependData.AddMainAsset(AssetDatabase.GetAssetPath(selObj.GetInstanceID()), true);
            dependData.FindMainDependAsset();
            dependData.Save();

            string[] shaders = dependData.GetUsedShader();
            foreach(var shader in shaders)
            {
                Debug.Log(shader);
            }


            //string[] assets = AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(selObj.GetInstanceID()),false);
            //Array.ForEach(assets, (asset) =>
            //{
            //    //Debug.Log(asset);

            //    UnityObject uObj = AssetDatabase.LoadAssetAtPath(asset, typeof(UnityObject));

            //    //if(uObj.GetType() != typeof(MonoScript))
            //    {
            //        Debug.Log($"path = {asset},type = {uObj.GetType().Name}");
            //    }
            //});
        }
    }
}
