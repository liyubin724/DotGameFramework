using System;
using UnityEngine;

namespace Dot.Core.World
{
    [Serializable]
    public class StaticObject
    {
        public string guid = string.Empty;
        public string assetPath = string.Empty;

        public Bounds bounds;

        public Vector3 position = Vector3.zero;
        public Quaternion quaternion = Quaternion.identity;

        public StaticObjectImportance objectImportance = StaticObjectImportance.Normal;

        public StaticObjectLightmap objectLightmap = null;
    }
}
