using Dot.Core.Entity;
using Dot.Core.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Battle.Entity
{
    public class BulletPhysicsController : EntityPhysicsController
    {
        protected override void OnSendTriggerEnter(EventData data)
        {
            GameObject targetGO = data.GetValue<GameObject>(0);
            EntityObject targetEntity = data.GetValue<EntityObject>(1);
            if(targetEntity!=null)
            {

            }
        }
    }
}
