using Dot.Core.Logger;
using UnityEditor;
using UnityEngine;

namespace Dot.Core.World
{
    public class WorldObjectBehaviour : MonoBehaviour
    {
        public MeshRenderer[] meshRenderers = new MeshRenderer[0];

        public void SetStaticObjectLightmap(StaticObjectRendererLightmap[] lightmaps)
        {
            if(lightmaps == null)
            {
                DebugLogger.LogError("");
                return;
            }
            if(meshRenderers == null)
            {
                DebugLogger.LogError("");
                return;
            }

            if(lightmaps.Length!=meshRenderers.Length)
            {
                DebugLogger.LogError("");
                return;
            }

            for(int i = 0;i<lightmaps.Length;++i)
            {
                SetLightmap(lightmaps[i]);
            }
        }

        private void SetLightmap(StaticObjectRendererLightmap lightmap)
        {
            if (lightmap.rendererIndex >= 0 && lightmap.rendererIndex < meshRenderers.Length)
            {
                Renderer renderer = meshRenderers[lightmap.rendererIndex];
                if (renderer != null)
                {
                    renderer.lightmapIndex = lightmap.lightmapIndex;
                    renderer.lightmapScaleOffset = lightmap.lightmapOffset;
                }
            }
        }
    }
}
