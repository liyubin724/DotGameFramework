using Dot.Core.Asset;
using Dot.Core.Event;
using Dot.Core.Logger;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Entity.Controller
{
    public class EntitySkeletonController : AEntityController
    {
        private string skeletonAddress = null;
        private GameObject skeletonGO = null;
        private AssetHandle skeletonAssetHandle = null;
        private NodeBehaviour nodeBehaviour = null;
        
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

        private void OnSkeletonLoadFinish(string address,UnityObject uObj)
        {
            skeletonAssetHandle = null;
            skeletonGO = uObj as GameObject;
            if(skeletonGO == null)
            {
                DebugLogger.LogError("EntitySkeletonController::OnSkeletonLoadFinish->skeleton is null");
                return;
            }

            EntityViewController viewController = entity.GetController<EntityViewController>(EntityControllerConst.VIEW_INDEX);
            if (viewController != null)
            {
                VirtualView view = viewController.GetView<VirtualView>();
                if(view!=null)
                {
                    skeletonGO.transform.SetParent(view.RootTransform, false);
                    return;
                }
            }

            skeletonGO.transform.SetParent(context.EntityRootTransfrom, false);
        }

        protected override void AddEventListeners()
        {
            Dispatcher.RegisterEvent(EntityInnerEventConst.SKELETON_ADD_ID, OnSkeletonAdd);
            Dispatcher.RegisterEvent(EntityInnerEventConst.SKELETON_REMOVE_ID, OnSkeletonRemove);
        }

        protected override void RemoveEventListeners()
        {
            Dispatcher.UnregisterEvent(EntityInnerEventConst.SKELETON_ADD_ID, OnSkeletonAdd);
            Dispatcher.UnregisterEvent(EntityInnerEventConst.SKELETON_REMOVE_ID, OnSkeletonRemove);
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

        public BindNodeData GetBindNodeData(string nodeName)
        {
            NodeBehaviour nodeBeh = GetNodeBehaviour();
            if(nodeBeh!=null)
            {
                return nodeBeh.GetBindNode(nodeName);
            }
            return null;
        }

        private NodeBehaviour GetNodeBehaviour()
        {
            if(nodeBehaviour == null && skeletonGO!=null)
            {
                nodeBehaviour = skeletonGO.GetComponent<NodeBehaviour>();
            }
            return nodeBehaviour;
        }
    }
}
