using Dot.Core.Logger;
using UnityEngine;

namespace Dot.Core.World
{
    public class StaticObjectBehaviour : MonoBehaviour
    {
        public string guid = string.Empty;
        public StaticObjectImportance objectImportance = StaticObjectImportance.Normal;
        public Bounds bounds;

        public MeshRenderer[] meshRenderers = new MeshRenderer[0];

        public StaticObjectBehaviour[] childBehaviours = new StaticObjectBehaviour[0];

        public void SetLightmap(StaticObjectLightmap lightmap)
        {
            SetObjectLightMap(this, lightmap,0);

            if(childBehaviours!=null && childBehaviours.Length>0)
            {
                int startIndex = meshRenderers.Length;
                foreach(var objBeh in childBehaviours)
                {
                    SetObjectLightMap(objBeh, lightmap, startIndex);
                    startIndex += objBeh.meshRenderers.Length;
                }
            }
        }

        private void SetObjectLightMap(StaticObjectBehaviour objectBehaviour, StaticObjectLightmap lightmap, int startIndex)
        {
            if (objectBehaviour == null || objectBehaviour.meshRenderers == null || objectBehaviour.meshRenderers.Length == 0)
            {
                DebugLogger.LogError("");
                return;
            }

            if (objectBehaviour.meshRenderers.Length > lightmap.rendererLightmaps.Length - startIndex)
            {
                DebugLogger.LogError("");
                return;
            }

            for (int i = 0; i < meshRenderers.Length; ++i)
            {
                StaticObjectRendererLightmap rendererLightmap = lightmap.rendererLightmaps[startIndex + i];
                MeshRenderer meshRenderer = objectBehaviour.meshRenderers[i];

                SetRendererLightmap(meshRenderer, rendererLightmap);
            }
        }

        private void SetRendererLightmap(MeshRenderer renderer, StaticObjectRendererLightmap lightmap)
        {
            if (renderer != null)
            {
                renderer.lightmapIndex = lightmap.lightmapIndex;
                renderer.lightmapScaleOffset = lightmap.lightmapOffset;
            }
        }
    }
}
