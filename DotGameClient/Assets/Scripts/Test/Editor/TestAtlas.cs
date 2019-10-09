using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using UnityEditor.U2D;

public class TestAtlas
{
    [MenuItem("Test/Test")]
    public static void TestAtlasF()
    {
        SpriteAtlas atlas = Selection.activeObject as SpriteAtlas;
        UnityEngine.Object[] objs = atlas.GetPackables();
        foreach(var obj in objs)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            Debug.Log("Type = " + obj.GetType().ToString()+"   path = "+path );
        }
    }
}
