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
        public long ParentUniqueID { get; set; } = 0;

        private Dictionary<int, AEntityController> controllerDic = new Dictionary<int, AEntityController>();
        private EventDispatcher entityDispatcher = new EventDispatcher();
        public EventDispatcher Dispatcher { get => entityDispatcher;}

        private EntityObject parentEntity = null;
        public void SetParent(EntityObject parent)
        {
            parentEntity = parent;
        }

        private Dictionary<int, List<EntityObject>> categroyChildrenDic = new Dictionary<int, List<EntityObject>>();
        public void AddChild(EntityObject child)
        {
            if(!categroyChildrenDic.TryGetValue(child.Category,out List<EntityObject> children))
            {
                children = new List<EntityObject>();
                categroyChildrenDic.Add(child.Category, children);
            }
            children.Add(child);
        }

        public EntityObject[] GetChildren(int category)
        {
            if (categroyChildrenDic.TryGetValue(category, out List<EntityObject> children))
            {
                return children.ToArray();
            }
            return null;
        }

        public void RemoveChild(EntityObject entity)
        {
            if (categroyChildrenDic.TryGetValue(entity.Category, out List<EntityObject> children))
            {
                children.Remove(entity);
            }
        }

        public void DoUpdate(float deltaTime)
        {
            foreach(var kvp in controllerDic)
            {
                kvp.Value?.DoUpdate(deltaTime);
            }
        }

        public void SendEvent(int eventID, params object[] values)
        {
            entityDispatcher.TriggerEvent(eventID, 0, values);
        }

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

        private void RemoveFromParent()
        {
            if(parentEntity!=null)
            {
                parentEntity.RemoveChild(this);
            }
        }
    }
}
