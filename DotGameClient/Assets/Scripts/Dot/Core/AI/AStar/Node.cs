using Dot.Core.Pool;
using System;

namespace Dot.Core.AI.AStar
{
    public enum NodeStatus
    {
        None,
        Open,
        Close,
    }

    public class Node : IComparable, IObjectPoolItem
    {
        public int Index { get; set; } = -1;
        public int X { get; set; } = -1;
        public int Y { get; set; } = -1;
        public int Px { get; set; } = -1;
        public int Py { get; set; } = -1;

        public NodeStatus Status { get; set; } = NodeStatus.None;

        private float f, g, h = 0;
        
        public float G
        {
            get
            {
                return g;
            }
            set
            {
                g = value;
                f = g + h;
            }
        }

        public float H
        {
            get
            {
                return h;
            }
            set
            {
                h = value;
                f = g + h;
            }
        }

        public float F { get => f; }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return -1;
            Node node = obj as Node;
            if (F > node.F)
                return 1;
            else if (F < node.F)
                return -1;
            else
                return 0;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Node node = obj as Node;
            return node.X == X && node.Y == Y;
        }
        
        public void OnNew()
        {
            
        }

        public void OnRelease()
        {
            g = h = f = 0;
            X = Y = Px = Py = -1;
        }

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }
    }


    public static class NodeFactroy
    {
        private static ObjectPool<Node> nodePool = null;

        public static void Init(int poolCount = 1000)
        {
            if(nodePool == null)
            {
                nodePool = new ObjectPool<Node>(poolCount);
            }
        }

        public static Node GetNode() => nodePool.Get();
        public static void ReleaseNode(Node node) => nodePool.Release(node);
    }
}
