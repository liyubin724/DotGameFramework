using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace DotEditor.Core.Prefab
{
    public static class NestPrefabMissingUtil
    {
        public static bool IsMissing(string assetPath)
        {
            PrefabUtil.OpenPrefabStage(assetPath);

            PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();
            GameObject rootGO = stage.prefabContentsRoot;
            Transform[] transforms = rootGO.GetComponentsInChildren<Transform>();
            foreach(var t in transforms)
            {
                if(t.name.IndexOf("Missing Prefab")>=0)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
