using Dot.Core.Event;
using Dot.Core.Logger;
using System;

namespace Dot.Core.Entity
{
    public class EntityObject
    {
        public long UniqueID { get; set; }
        public int Category { get; set; }
        public string Name { get; set; }

        private AEntityController[] controllers = new AEntityController[0];
        private EventDispatcher entityDispatcher = new EventDispatcher();

        public EntityObject()
        {
            controllers = new AEntityController[EntityControllerConst.MAX_INDEX];
        }

        public AEntityController this[int controllerIndex]
        {
            get
            {
                return controllers[controllerIndex];
            }
            set
            {
                if (controllers[controllerIndex] == null)
                {
                    DebugLogger.LogError("");
                    return;
                }
                controllers[controllerIndex] = value;
                if(value !=null)
                {
                    value.Dispatcher = entityDispatcher;
                }
            }
        }

        public void DoUpdate(float deltaTime)
        {
            Array.ForEach(controllers, (controller) =>
            {
                controller?.DoUpdate(deltaTime);
            });
        }

        public void SendEvent(int eventID, params object[] values)
        {
            entityDispatcher.TriggerEvent(eventID, 0, values);
        }

        public void AddController(int index, AEntityController controller)
        {
            if(this[index] == null)
            {
                this[index] = controller;
            }
        }

        public T GetController<T>(int index) where T:AEntityController => (T)this[index];

        public void DoReset()
        {

        }
    }
}
