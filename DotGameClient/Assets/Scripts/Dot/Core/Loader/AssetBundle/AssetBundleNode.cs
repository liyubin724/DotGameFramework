using Dot.Core.Pool;
using System;
using System.Collections.Generic;
using UnityEngine;
using SystemObject = System.Object;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public class AssetNode : IObjectPoolItem
    {
        private string assetPath = null;
        private BundleNode bundleNode = null;
        private List<WeakReference> weakAssets = new List<WeakReference>();

        private int loadCount = 0;
        public void RetainLoadCount() => ++loadCount;
        public void ReleaseLoadCount() => --loadCount;

        public AssetNode() { }
        public void InitNode(string path,BundleNode node)
        {
            assetPath = path;
            bundleNode = node;
            bundleNode.RetainRefCount();
        }

        public bool IsDone
        {
            get
            {
                return bundleNode.IsDone;
            }
        }

        public bool IsAlive()
        {
            if(!IsDone || loadCount>0 || bundleNode.IsScene)
            {
                return true;
            }

            foreach(var weakAsset in weakAssets)
            {
                if (!IsNull(weakAsset.Target))
                {
                    return true;
                }
            }
            return false;
        }

        public UnityObject GetAsset()
        {
            UnityObject asset = bundleNode.GetAsset(assetPath);
            if(bundleNode.IsScene)
            {
                return asset;
            }
            weakAssets.Add(new WeakReference(asset, false));
            return asset;
        }

        public UnityObject GetInstance()
        {
            UnityObject asset = bundleNode.GetAsset(assetPath);
            if (bundleNode.IsScene)
            {
                Debug.LogError("AssetNode::GetInstance->bundle is scene.can't Instance it");
                return asset;
            }

            UnityObject instance = UnityObject.Instantiate(asset);
            Resources.UnloadAsset(asset);
            AddInstance(instance);
            return instance;
        }

        public void AddInstance(UnityObject uObj)
        {
            bool isSet = false;
            for (int i = 0; i < weakAssets.Count; ++i)
            {
                if (IsNull(weakAssets[i].Target))
                {
                    weakAssets[i].Target = uObj;
                    isSet = true;
                    break;
                }
            }

            if(!isSet)
            {
                weakAssets.Add(new WeakReference(uObj,false));
            }
        }

        private bool IsNull(SystemObject sysObj)
        {
            if (sysObj == null || sysObj.Equals(null))
            {
                return true;
            }

            return false;
        }

        public void OnNew() { }
        public void OnRelease()
        {
            assetPath = null;
            bundleNode.ReleaseRefCount();
            bundleNode = null;
            weakAssets.Clear();
            loadCount = 0;
        }
    }

    public class BundleNode : IObjectPoolItem
    {
        private string bundlePath;
        private int refCount;
        private bool isDone = false;
        private bool isSetAssetBundle = false;
        private AssetBundle assetBundle = null;
        private List<BundleNode> directDependNodes = new List<BundleNode>();

        public BundleNode() { }
        public void InitNode(string path)
        {
            bundlePath = path;
        }

        public void SetAssetBundle(AssetBundle bundle)
        {
            assetBundle = bundle;
            isSetAssetBundle = true;
        }

        public void AddDependNode(BundleNode node)
        {
            directDependNodes.Add(node);
            node.RetainRefCount();
        }

        public int RefCount { get => refCount;}
        public void RetainRefCount() => ++refCount;
        public void ReleaseRefCount()
        {
            --refCount;
            if(refCount == 0)
            {
                for(int i =0;i<directDependNodes.Count;++i)
                {
                    directDependNodes[i].ReleaseRefCount();
                }
            }
        }

        public bool IsDone
        {
            get
            {
                if(isDone)
                {
                    return true;
                }

                if (!isSetAssetBundle)
                {
                    return false;
                }
                for (int i = 0; i < directDependNodes.Count; ++i)
                {
                    if(!directDependNodes[i].IsDone)
                    {
                        return false;
                    }
                }
                isDone = true;
                return isDone;
            }
        }

        public bool IsScene
        {
            get
            {
                if(assetBundle!=null)
                {
                    return assetBundle.isStreamedSceneAssetBundle;
                }else if(!isSetAssetBundle)
                {
                    Debug.LogError("BundleNode::IsScene->AssetBundle has not been set,you should call IsDone at first");
                }else
                {
                    Debug.LogError("BundleNode::IsScene->AssetBundle Load failed");
                }
                
                return false;
            }
        }

        public UnityObject GetAsset(string assetPath)
        {
            return IsScene ? assetBundle : assetBundle?.LoadAsset(assetPath);
        }

        public void OnNew() { }
        public void OnRelease()
        {
            bundlePath = null;
            isSetAssetBundle = false;
            isDone = false;
            assetBundle?.Unload(true);
            assetBundle = null;
            directDependNodes.Clear();
            refCount = 0;
        }
    }
}
