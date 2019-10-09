using UnityEngine;

namespace Dot.Tools
{
    public static class ToolUtil
    {
        public static void AddDrawMeshFilterBounds(GameObject gObj)
        {
            if(gObj!=null)
            {
                MeshFilter meshFilter = gObj.GetComponent<MeshFilter>();
                if(meshFilter!=null)
                {
                    DrawMeshFilterBounds drawMeshFilterBounds = gObj.AddComponent<DrawMeshFilterBounds>();
                    drawMeshFilterBounds.filter = meshFilter;
                }
            }
        }

        public static void RemoveDrawMeshFilterBounds(GameObject gObj)
        {
            if (gObj != null)
            {
                DrawMeshFilterBounds drawMeshFilterBounds = gObj.GetComponent<DrawMeshFilterBounds>();
                if(drawMeshFilterBounds!=null)
                {
                    UnityEngine.Object.Destroy(drawMeshFilterBounds);
                }
            }
        }

    }
}
