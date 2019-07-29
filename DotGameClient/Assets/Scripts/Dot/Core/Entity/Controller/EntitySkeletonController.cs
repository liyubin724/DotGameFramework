using Dot.Core.Asset;
using Dot.Core.Event;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Entity.Controller
{
    public class EntitySkeletonController : AEntityController
    {
        private string skeletonAddress = null;
        private GameObject skeletonGO = null;
        private AssetHandle skeletonAssetHandle = null;
        private EntityNodeBehaviour nodeBehaviour = null;
        
        public EntitySkeletonController(EntityObject entity) : base(entity)
        {
        }

        public bool HasSkeleton() => skeletonGO == null;

        public void AddSkeleton(string skeletonAddress)
        {
            if(this.skeletonAddress == skeletonAddress)
            {
                return;
            }
            this.skeletonAddress = skeletonAddress;
            if(skeletonAssetHandle!=null)
            {
                skeletonAssetHandle.Release();
                skeletonAssetHandle = null;
            }
            skeletonAssetHandle = AssetManager.GetInstance().InstanceAssetAsync(skeletonAddress, OnSkeletonLoadFinish, null);
        }

        public void RemoveSkeleton()
        {
            if(skeletonGO!=null)
            {
                UnityObject.Destroy(skeletonGO);
                skeletonGO = null;
            }else if(skeletonAssetHandle!=null)
            {
                skeletonAssetHandle.Release();
                skeletonAssetHandle = null;
            }
            skeletonAddress = null;
            nodeBehaviour = null;
        }

        public override void DoReset()
        {
            
        }

        private void OnSkeletonLoadFinish(string address,UnityObject uObj)
        {
            skeletonAssetHandle = null;
            skeletonGO = uObj as GameObject;

            if(Entity.View!=null)
            {
                VirtualView vView = Entity.View as VirtualView; 
                if(vView!=null)
                {
                    skeletonGO.transform.SetParent(vView.RootTransform, false);
                }
            }
        }

        protected override void AddEventListeners()
        {
            Dispatcher.RegisterEvent(EntityEventConst.SKELETON_ADD_ID, OnSkeletonAdd);
            Dispatcher.RegisterEvent(EntityEventConst.SKELETON_REMOVE_ID, OnSkeletonRemove);
        }

        protected override void RemoveEventListeners()
        {
            Dispatcher.UnregisterEvent(EntityEventConst.SKELETON_ADD_ID, OnSkeletonAdd);
            Dispatcher.UnregisterEvent(EntityEventConst.SKELETON_REMOVE_ID, OnSkeletonRemove);
        }

        private void OnSkeletonAdd(EventData eventData)
        {
            string address = eventData.GetValue<string>();
            AddSkeleton(address);
        }

        private void OnSkeletonRemove(EventData eventData)
        {
            RemoveSkeleton();
        }

        public EntityBindNodeData GetBindNodeData(string nodeName)
        {
            EntityNodeBehaviour nodeBeh = GetNodeBehaviour();
            if(nodeBeh!=null)
            {
                return nodeBeh.GetBindNode(nodeName);
            }
            return null;
        }

        private EntityNodeBehaviour GetNodeBehaviour()
        {
            if(nodeBehaviour == null && skeletonGO!=null)
            {
                nodeBehaviour = skeletonGO.GetComponent<EntityNodeBehaviour>();
            }
            return nodeBehaviour;
        }
    }
}
