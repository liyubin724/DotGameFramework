using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dot.Core.World
{
    public abstract class AQuadTreeObject
    {
        public abstract Rect ObjectRect { get; }
        public abstract string Guid { get; }
    }
}
