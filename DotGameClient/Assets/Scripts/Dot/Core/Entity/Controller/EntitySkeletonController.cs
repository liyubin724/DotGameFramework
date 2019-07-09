using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Dot.Core.Entity.Controller
{
    public class EntitySkeletonController : EntityController
    {
        private string skeletonPath = "";
        private GameObject skeletonGO = null;

        public EntitySkeletonController(EntityObject entityObj) : base(entityObj)
        {
        }

        public void LoadSkeleton(string skeleton)
        {
            skeletonPath = skeleton;
            Addressables.LoadAssetAsync<GameObject>(skeleton).Completed += LoadSkeletonCompleted;
        }

        public void UnloadSkeleton()
        {

        }

        public override void OnDispose()
        {
            
        }

        public override void OnReset()
        {
            
        }

        private void LoadSkeletonCompleted(AsyncOperationHandle<GameObject> obj)
        {
            GameObject gObj = obj.Result as GameObject;
            skeletonGO = GameObject.Instantiate<GameObject>(gObj);
            skeletonGO.transform.SetParent(Entity.GetTransform(), false);

            Addressables.Release(obj.Result);
        }

        protected override void AddEventListeners()
        {
            
        }

        protected override void RemoveEventListeners()
        {
            
        }
    }
}
