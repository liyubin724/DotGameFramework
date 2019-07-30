using Dot.Core.Event;
using UnityEngine;

namespace Dot.Core.Entity
{
    public class VirtualView : AEntityView
    {
        public GameObject RootGameObject{get; private set;}
        public Transform RootTransform{get; private set;}
        public Vector3 Position { get => RootTransform.position; }
        public Vector3 Direction { get => RootTransform.forward; }

        public override bool Enable
        {
            get
            {
                return RootGameObject.activeSelf;
            }
            set
            {
                RootGameObject.SetActive(value);
            }
        }

        public EventDispatcher Dispatcher { get; private set; }

        public VirtualView(string name, EventDispatcher dispatcher) : this(name, null, dispatcher)
        {
        }

        public VirtualView(string name, Transform parent, EventDispatcher dispatcher)
        {
            RootGameObject = new GameObject(name);
            RootTransform = RootGameObject.transform;

            Dispatcher = dispatcher;

            if (parent != null)
            {
                RootTransform.SetParent(parent, false);
            }
        }

        public void SetLayer(int layer,bool includeChildren)=> SetLayer(RootGameObject, layer, includeChildren);
        private void SetLayer(GameObject gObj,int layer,bool includeChildren)
        {
            gObj.layer = layer;
            if(includeChildren)
            {
                foreach(Transform tran in gObj.transform)
                {
                    SetLayer(tran.gameObject, layer, includeChildren);
                }
            }
        }

        public override void AddListener()
        {
            Dispatcher?.RegisterEvent(EntityInnerEventConst.POSITION_ID, OnPosition);
            Dispatcher?.RegisterEvent(EntityInnerEventConst.DIRECTION_ID, OnDirection);
        }

        public override void RemoveListener()
        {
            Dispatcher?.UnregisterEvent(EntityInnerEventConst.POSITION_ID, OnPosition);
            Dispatcher?.UnregisterEvent(EntityInnerEventConst.DIRECTION_ID, OnDirection);
        }

        protected virtual void OnPosition(EventData eventData)
        {
            RootTransform.position = eventData.GetValue<Vector3>(0);
        }

        protected virtual void OnDirection(EventData eventData)
        {
            RootTransform.forward = eventData.GetValue<Vector3>(0);
        }
    }
}
