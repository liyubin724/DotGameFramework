using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dot.Core.World
{
    public class QuadTreeNode
    {
        private Rect rect;

        private QuadTreeNode parent;
        private QuadTreeNode childTL;
        private QuadTreeNode childTR;
        private QuadTreeNode childBL;
        private QuadTreeNode childBR;

        private List<AQuadTreeObject> objects = new List<AQuadTreeObject>();
        
        public QuadTreeNode(Rect r)
        {
            rect = r;
        }

        public QuadTreeNode(float x,float y,float width,float height):this(new Rect(x,y,width,height))
        {
        }

        public QuadTreeNode(QuadTreeNode pNode,Rect rect):this(rect)
        {
            parent = pNode;
        }

        public bool IsLeaf
        {
            get
            {
                return childBL == null && childBR == null && childTL == null && childTR == null;
            }
        }

        internal void AddData(AQuadTreeObject qtObj)
        {

        }

    }
}
