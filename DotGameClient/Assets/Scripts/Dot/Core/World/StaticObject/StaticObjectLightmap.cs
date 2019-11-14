using System;

namespace Dot.Core.World
{
    [Serializable]
    public class StaticObjectLightmap
    {
        public StaticObjectRendererLightmap[] rendererLightmaps = new StaticObjectRendererLightmap[0];

        public StaticObjectLightmap[] childLightmaps = new StaticObjectLightmap[0];
    }
}
