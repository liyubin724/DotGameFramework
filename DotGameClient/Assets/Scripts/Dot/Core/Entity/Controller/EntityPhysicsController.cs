using Dot.Core.Event;

namespace Dot.Core.Entity
{
    public class EntityPhysicsController : AEntityController
    {
        protected override void AddEventListeners()
        {
            entity.RegisterEvent(EntityInnerEventConst.TRIGGER_ENTER_ID, OnSendTriggerEnter);
        }

        protected override void RemoveEventListeners()
        {
            entity.UnregisterEvent(EntityInnerEventConst.TRIGGER_ENTER_ID, OnSendTriggerEnter);
        }

        protected virtual void OnSendTriggerEnter(EventData data)
        {

        }
    }
}
