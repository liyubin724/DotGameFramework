using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dot.Core.World.QuadTree
{
    [Serializable]
    public class WorldObjectData
    {
        public string uid;
        public string resPath;
        public Bounds bounds;
        public Vector3 position = Vector3.zero;
        public Quaternion quaternion = Quaternion.identity;
    }
}
