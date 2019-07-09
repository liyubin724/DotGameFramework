using Dot.Core.Entity.Controller;
using Dot.Core.Event;
using UnityEngine;
using SystemObject = System.Object;

namespace Dot.Core.Entity
{
    public class EntityObject
    {
        public int ID { get; set; }

        private Transform cachedTransform = null;
        private GameObject cachedGameObject = null;

        private EntityController[] controllers = new EntityController[0];
        private EventDispatcher eventDispatcher = new EventDispatcher();

        public EntityObject(int id)
        {
            ID = id;
            controllers = new EntityController[(int)EntityControllerType.Max];
        }

        public void CreateVirtualRoot()
        {
            cachedGameObject = new GameObject($"Entity_{ID}");
            cachedTransform = cachedGameObject.transform;
        }

        public Transform GetTransform()
        {
            return cachedTransform;
        }

        public GameObject GetGameObject()
        {
            return cachedGameObject;
        }

        public virtual void DoInit() { }
        public virtual void DoStart() { }
        public virtual void DoReset()
        {
            eventDispatcher.DoReset();
        }
        public virtual void DoDispose(){}

        public void SendEvent(int eventID,params object[] values)
        {
            eventDispatcher?.TriggerEvent(eventID,0,values);
        }

        public void RegistEvent(int eventID,EventHandler handler)
        {
            eventDispatcher?.RegisterEvent(eventID, handler);
        }

        public void UnregisterEvent(int eventID, EventHandler handler)
        {
            eventDispatcher?.UnregisterEvent(eventID, handler);
        }

        public void AddController(EntityControllerType controllerType)
        {
            if (controllers[(int)controllerType] != null)
            {
                return;
            }
        }
    }
}
