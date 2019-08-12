using Dot.Core.Entity.Data;
using Dot.Core.Event;
using Dot.Core.Logger;
using System.Collections.Generic;

namespace Dot.Core.Entity
{
    public class EntityObject
    {
        public long UniqueID { get; set; }
        public int Category { get; set; }
        public string Name { get; set; }
        public EntityBaseData EntityData { get; set; }

        private Dictionary<int, AEntityController> controllerDic = new Dictionary<int, AEntityController>();
        private EventDispatcher entityDispatcher = new EventDispatcher();
        public EventDispatcher Dispatcher { get => entityDispatcher; }
       
        public void DoUpdate(float deltaTime)
        {
            foreach(var kvp in controllerDic)
            {
                kvp.Value?.DoUpdate(deltaTime);
            }
        }

        public void SendEvent(int eventID, params object[] values)=> entityDispatcher.TriggerEvent(eventID, 0, values);
        public void RegisterEvent(int eventID,EventHandler handler)=> entityDispatcher.RegisterEvent(eventID, handler);
        public void UnregisterEvent(int eventID, EventHandler handler) => entityDispatcher.UnregisterEvent(eventID, handler);

        public T GetController<T>(int index) where T : AEntityController
        {
            if (controllerDic.TryGetValue(index, out AEntityController controller))
            {
                return (T)controller;
            }

            return null;
        }

        public bool HasController(int index) => controllerDic.ContainsKey(index);
        
        public void AddController(int index,AEntityController controller)
        {
            if (controller == null)
            {
                DebugLogger.LogError("EntityObject::this[index]-> value is null");
                return;
            }
            if (!controllerDic.ContainsKey(index))
            {
                controllerDic.Add(index, controller);
            }else
            {
                DebugLogger.LogError("EntityObject::this[index]->controller has been added.if you want to replace it,please use ReplaceController instead");
            }
        }

        public AEntityController ReplaceController(int index,AEntityController controller)
        {
            AEntityController replacedController = RemoveController(index);
            AddController(index, controller);
            return replacedController;
        }

        public AEntityController RemoveController(int index)
        {
            if (controllerDic.TryGetValue(index, out AEntityController controller))
            {
                controllerDic.Remove(index);
            }
            return controller;
        }

        public void RemoveAllController(out int[] indexes,out AEntityController[] controllers)
        {
            indexes = new int[controllerDic.Count];
            controllers = new AEntityController[controllerDic.Count];
            int index = 0;
            foreach(var kvp in controllerDic)
            {
                indexes[index] = kvp.Key;
                controllers[index] = kvp.Value;
                ++index;
            }

            controllerDic.Clear();
        }

        public virtual void DoReset()
        {

        }

        public virtual void DoDestroy()
        {

        }
    }
}
