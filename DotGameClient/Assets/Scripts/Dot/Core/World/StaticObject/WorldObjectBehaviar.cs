using System;
using UnityEngine;

namespace Dot.Core.World
{
    



    [Serializable]
    public class WorldStaticObjectBehaviar
    {
        [SerializeField]
        private Renderer[] renderers = new Renderer[0];

        public void SetLightmapData(WorldStaticObjectLightmapData lightmapData)
        {
            if(lightmapData.rendererIndex >= 0 && lightmapData.rendererIndex < renderers.Length)
            {
                Renderer renderer = renderers[lightmapData.rendererIndex];
                if(renderer!=null)
                {
                    renderer.lightmapIndex = lightmapData.lightmapIndex;
                    renderer.lightmapScaleOffset = lightmapData.lightmapOffset;
                }
            }
        }

        public WorldStaticObjectLightmapData[] GetLightmapDatas()
        {
            if(renderers == null || renderers.Length == 0)
            {
                return null;
            }

            WorldStaticObjectLightmapData[] datas = new WorldStaticObjectLightmapData[renderers.Length];
            for(int i =0;i<renderers.Length;i++)
            {
                WorldStaticObjectLightmapData data = new WorldStaticObjectLightmapData();
                datas[i] = data;

                data.rendererIndex = i;
                data.lightmapIndex = renderers[i].lightmapIndex;
                data.lightmapOffset = renderers[i].lightmapScaleOffset;
            }
            return datas;
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            Vector3 boundPoint1 = bounds.min;
            Vector3 boundPoint2 = bounds.max;
            Vector3 boundPoint3 = new Vector3(boundPoint1.x, boundPoint1.y, boundPoint2.z);
            Vector3 boundPoint4 = new Vector3(boundPoint1.x, boundPoint2.y, boundPoint1.z);
            Vector3 boundPoint5 = new Vector3(boundPoint2.x, boundPoint1.y, boundPoint1.z);
            Vector3 boundPoint6 = new Vector3(boundPoint1.x, boundPoint2.y, boundPoint2.z);
            Vector3 boundPoint7 = new Vector3(boundPoint2.x, boundPoint1.y, boundPoint2.z);
            Vector3 boundPoint8 = new Vector3(boundPoint2.x, boundPoint2.y, boundPoint1.z);

            // rectangular cuboid
            // top of rectangular cuboid (6-2-8-4)
            Color gizmosColor = Gizmos.color;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(boundPoint6, boundPoint2);
            Gizmos.DrawLine(boundPoint2, boundPoint8);
            Gizmos.DrawLine(boundPoint8, boundPoint4);
            Gizmos.DrawLine(boundPoint4, boundPoint6);
            // bottom of rectangular cuboid (3-7-5-1)
            Gizmos.DrawLine(boundPoint3, boundPoint7);
            Gizmos.DrawLine(boundPoint7, boundPoint5);
            Gizmos.DrawLine(boundPoint5, boundPoint1);
            Gizmos.DrawLine(boundPoint1, boundPoint3);
            // legs (6-3, 2-7, 8-5, 4-1)
            Gizmos.DrawLine(boundPoint6, boundPoint3);
            Gizmos.DrawLine(boundPoint2, boundPoint7);
            Gizmos.DrawLine(boundPoint8, boundPoint5);
            Gizmos.DrawLine(boundPoint4, boundPoint1);
        }

        private void OnDrawGizmosSelected()
        {
            
        }

#endif

    }
}
