using System;
using UnityEngine;

namespace Dot.Core.World
{
    [Serializable]
    public class StaticObjectRendererLightmap
    {
        public int rendererIndex;
        public int lightmapIndex;
        public Vector4 lightmapOffset;
    }
}
