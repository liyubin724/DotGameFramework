using Dot.Core.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Dot.Tests
{
    public class TestPrefabLightmap : MonoBehaviour
    {
        public GameObject instance = null;
        public GameObject prefab = null;
        public WorldStaticObjectLightmapData data;

        [MenuItem("Test/Tt")]
        public static void SetPrefab()
        {
            TestPrefabLightmap tpl = Selection.activeGameObject.GetComponent<TestPrefabLightmap>();
            tpl.data = tpl.instance.GetComponent<WorldStaticObjectBehaviar>().GetLightmapDatas()[0];
            EditorUtility.SetDirty(tpl);
        }

        void Start()
        {
            WorldStaticObjectBehaviar beh = Instantiate(prefab).GetComponent<WorldStaticObjectBehaviar>();
            beh.SetLightmapData(data);


        }

        // Update is called once per frame
        void Update()
        {

        }


    }
}


