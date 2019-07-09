using Dot.Core.Logger;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Util
{
    public static class GameObjectHandler
    {
        public static GameObject Instantiate(string path,GameObject obj)
        {
            return Instantiate<GameObject>(path, obj);
        }

        public static T Instantiate<T>(string path,T obj) where T:UnityObject
        {
            T uobj = null;
#if DEBUG
            if(obj == null)
            {
                DebugLogger.LogError("GameObjectHandler::Instantiate->Template Obj is Null.path = " + path);
            }
#endif
#if ASSET_BUNDLE
            uobj = GameAsset.assetLoader.Instantiate<T>(path, obj);
#else
            uobj = UnityObject.Instantiate(obj);
#endif
            return uobj;
        }
    }
}
