using Dot.Core.Event;
using UnityEngine;
using UnityObject = UnityEngine.Object;

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

        public VirtualView(string name) : this(name, null)
        {
        }

        public VirtualView(string name, Transform parent)
        {
            RootGameObject = new GameObject(name);
            RootTransform = RootGameObject.transform;

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
            entity.RegisterEvent(EntityInnerEventConst.POSITION_ID, OnPosition);
            entity?.RegisterEvent(EntityInnerEventConst.DIRECTION_ID, OnDirection);
        }

        public override void RemoveListener()
        {
            entity.UnregisterEvent(EntityInnerEventConst.POSITION_ID, OnPosition);
            entity.UnregisterEvent(EntityInnerEventConst.DIRECTION_ID, OnDirection);
        }

        public override void DoDestroy()
        {
            UnityObject.Destroy(RootGameObject);

            base.DoDestroy();
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
