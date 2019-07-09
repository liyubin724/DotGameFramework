using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dot.Core.Entity
{
    public class EntityContext
    {
        private Transform entityRootTran = null;
        public EntityContext()
        {
            entityRootTran = new GameObject("Entity Root").transform;
        }

        public EntityObject CreateEntity()
        {
            EntityObject e = new EntityObject(0);
            e.CreateVirtualRoot();

            e.GetTransform().SetParent(entityRootTran, false);

            return null;
        }
    }
}
