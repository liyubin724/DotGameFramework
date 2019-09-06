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
        //private WeakReference weakAsset;
        //private List<WeakReference> weakInstances;
        private List<WeakReference> weakAssets = new List<WeakReference>();

        private int loadCount = 0;


        public void RetainLoadCount() => ++loadCount;
        public void ReleaseLoadCount() => --loadCount;

        public AssetNode() { }
        public void InitNode(string path,BundleNode node)
        {
            assetPath = path;
            bundleNode = node;
            //weakAsset = new WeakReference(node.GetAsset(assetPath),true);
        }

        public bool IsAlive()
        {
            if (loadCount > 0) return true;

            foreach(var weakAsset in weakAssets)
            {
                if (!IsNull(weakAsset.Target))
                {
                    return true;
                }
            }
            
            //if (!IsNull(weakAsset.Target)) return true;

            //if(weakInstances!=null)
            //{
            //    for(int i =weakInstances.Count-1;i>=0;--i)
            //    {
            //        if(!IsNull(weakInstances[i].Target))
            //        {
            //            return true;
            //        }
            //    }
            //}
            return false;
        }

        public UnityObject GetAsset()
        {
            UnityObject asset = bundleNode.GetAsset(assetPath);
            weakAssets.Add(new WeakReference(asset, false));
            return asset;
            //if (IsNull(weakAsset.Target))
            //{
            //    weakAsset.Target = bundleNode.GetAsset(assetPath);
            //}

            //return weakAsset.Target as UnityObject;
        }

        public UnityObject GetInstance()
        {
            UnityObject asset = bundleNode.GetAsset(assetPath);
            UnityObject instance = UnityObject.Instantiate(asset);
            Resources.UnloadAsset(asset);
            AddInstance(instance);
            return instance;
        }

        public void AddInstance(UnityObject uObj)
        {
            //if (weakInstances == null)
            //{
            //    weakInstances = new List<WeakReference>();
            //}

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
            //weakAsset = null;
            //weakInstances = null;
            weakAssets.Clear();
            loadCount = 0;
        }
    }

    public class BundleNode : IObjectPoolItem
    {
        private string bundlePath;
        private AssetBundle assetBundle;
        private int refCount;
        public int RefCount { get => refCount; set => refCount = value; }
        public void RetainRefCount() => ++refCount;
        public void ReleaseRefCount() => --refCount;
        
        public BundleNode() { }

        public void InitNode(string path,AssetBundle bundle)
        {
            bundlePath = path;
            assetBundle = bundle;
        }

        public UnityObject GetAsset(string assetPath)
        {
            return assetBundle.LoadAsset(assetPath);
        }

        public void OnNew() { }
        public void OnRelease()
        {
            bundlePath = null;
            assetBundle.Unload(true);
            assetBundle = null;
            refCount = 0;
        }
    }
}
