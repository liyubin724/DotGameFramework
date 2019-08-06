using Dot.Core.Event;
using Dot.Core.Generic;
using Dot.Core.Logger;
using Dot.Core.Util;
using Game.Entity;
using System.Collections.Generic;
using UnityEngine;
using SystemObject = System.Object;

namespace Dot.Core.Entity
{
    public partial class EntityContext
    {
        private Transform entityRootTran = null;
        public Transform EntityRootTransfrom { get => entityRootTran; }

        private UniqueIDCreator idCreator = new UniqueIDCreator();

        private static EntityContext instance = null;
        public static EntityContext GetInstance()
        {
            return instance;
        }
        
        
        public EntityContext()
        {
            instance = this;
            entityRootTran = DontDestroyHandler.CreateTransform("Entity Root");
            EntityBuilderFactory.RegisterEntityBuilder(this);
        }

        public void DoUpdate(float deltaTime)
        {
            foreach(var kvp in entityDic)
            {
                kvp.Value.DoUpdate(deltaTime);
            }
        }
        
        
    }


}
