using Dot.Core.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Core.Entity
{
    public class EntityPhysicsController : AEntityController
    {

        protected override void AddEventListeners()
        {
            Dispatcher.RegisterEvent(EntityInnerEventConst.TRIGGER_ENTER_SENDER_ID, OnSendTriggerEnter);
            Dispatcher.RegisterEvent(EntityInnerEventConst.TRIGGER_ENTER_RECEIVER_ID, OnReceiverTriggerEnter);
        }

        protected override void RemoveEventListeners()
        {
            Dispatcher.UnregisterEvent(EntityInnerEventConst.TRIGGER_ENTER_SENDER_ID, OnSendTriggerEnter);
            Dispatcher.UnregisterEvent(EntityInnerEventConst.TRIGGER_ENTER_RECEIVER_ID, OnReceiverTriggerEnter);
        }

        protected virtual void OnSendTriggerEnter(EventData data)
        {

        }

        protected virtual void OnReceiverTriggerEnter(EventData data)
        {

        }
    }
}
