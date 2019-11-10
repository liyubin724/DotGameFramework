using UnityEditor;
using UnityEngine;

namespace DotEditor.Core.Util
{
    public static class GameObjectUtil
    {
        public static bool IsLightmapStatic(GameObject go)
        {
            return GameObjectUtility.AreStaticEditorFlagsSet(go, StaticEditorFlags.LightmapStatic);
        }

        public static void SetLightmapStatic(GameObject go)
        {
            var flags = GameObjectUtility.GetStaticEditorFlags(go);
            flags |= StaticEditorFlags.LightmapStatic;

            GameObjectUtility.SetStaticEditorFlags(go, flags);
        }
        
    }
}
