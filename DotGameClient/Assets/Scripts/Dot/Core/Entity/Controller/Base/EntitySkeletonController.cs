using Dot.Core.Asset;
using Dot.Core.Event;
using Dot.Core.Logger;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using SystemObject = System.Object;

namespace Dot.Core.Entity.Controller
{
    public class EntitySkeletonController : AEntityController
    {
        private string skeletonAddress = null;
        private GameObject skeletonGO = null;
        private NodeBehaviour nodeBehaviour = null;

        private AssetHandle skeletonAssetHandle = null;
        
        public bool HasSkeleton() => skeletonGO == null;

        public void AddSkeleton(string skeletonAddress)
        {
            if(this.skeletonAddress == skeletonAddress)
            {
                return;
            }

            this.skeletonAddress = skeletonAddress;

            skeletonAssetHandle?.Release();
            skeletonAssetHandle = AssetLoader.GetInstance().InstanceAssetAsync(skeletonAddress, OnSkeletonLoadFinish, null,null);
        }

        public void RemoveSkeleton()
        {
            if(skeletonGO!=null)
            {
                UnityObject.Destroy(skeletonGO);
                skeletonGO = null;
            }
            skeletonAssetHandle?.Release();
            skeletonAssetHandle = null;
            skeletonAddress = null;
            nodeBehaviour = null;
        }

        private void OnSkeletonLoadFinish(string address,UnityObject uObj,SystemObject userData)
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
            entity.RegisterEvent(EntityInnerEventConst.SKELETON_ADD_ID, OnSkeletonAdd);
            entity.RegisterEvent(EntityInnerEventConst.SKELETON_REMOVE_ID, OnSkeletonRemove);
        }

        protected override void RemoveEventListeners()
        {
            entity.UnregisterEvent(EntityInnerEventConst.SKELETON_ADD_ID, OnSkeletonAdd);
            entity.UnregisterEvent(EntityInnerEventConst.SKELETON_REMOVE_ID, OnSkeletonRemove);
        }

        public override void DoReset()
        {
            RemoveSkeleton();

            base.DoReset();
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

        public BindNodeData GetBindNodeData(BindNodeType nodeType,int nodeIndex)
        {
            BindNodeData[] nodes = GetBindNodes(nodeType);
            if (nodes != null && nodeIndex >= 0 && nodeIndex < nodes.Length)
            {
                return nodes[nodeIndex];
            }
            return null;
        }

        public BindNodeData[] GetBindNodes(BindNodeType nodeType)
        {
            NodeBehaviour nodeBeh = GetNodeBehaviour();
            if (nodeBeh != null)
            {
                return nodeBeh.GetBindNodes(nodeType);
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
