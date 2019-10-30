using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dot.Core.World
{
    public class WorldStaticObject
    {
        public string guid = string.Empty;

        public string assetPath = string.Empty;

        public Vector3 position = Vector3.zero;
        public Quaternion quaternion = Quaternion.identity;

        public WorldObjectRendererLightmap[] rendererLightmaps = new WorldObjectRendererLightmap[0];

        [Serializable]
        public class WorldObjectRendererLightmap
        {
            public int rendererIndex;
            public int lightmapIndex;
            public Vector4 lightmapOffset;
        }
    }
}
