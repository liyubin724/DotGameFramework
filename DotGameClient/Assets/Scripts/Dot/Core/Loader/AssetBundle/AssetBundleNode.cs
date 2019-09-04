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
        private string assetPath;
        private BundleNode assetBundleNode;
        private WeakReference weakAsset;
        private List<WeakReference> weakInstances;

        public AssetNode() { }
        public void InitNode(string path,BundleNode node,UnityObject uObj)
        {
            assetPath = path;
            assetBundleNode = node;
            weakAsset = new WeakReference(uObj);
        }

        public bool IsAlive()
        {
            if (!IsNull(weakAsset.Target)) return true;

            if(weakInstances!=null)
            {
                for(int i =weakInstances.Count;i>=0;--i)
                {
                    if(!IsNull(weakInstances[i].Target))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public UnityObject GetAsset()
        {
            return weakAsset.Target as UnityObject;
        }

        public void AddInstance(UnityObject uObj)
        {
            if (weakInstances == null)
            {
                weakInstances = new List<WeakReference>();
            }

            bool isSet = false;
            for (int i = 0; i < weakInstances.Count; ++i)
            {
                if (IsNull(weakInstances[i].Target))
                {
                    weakInstances[i].Target = uObj;
                    isSet = true;
                    break;
                }
            }

            if(!isSet)
            {
                weakInstances.Add(new WeakReference(uObj));
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

        public void OnNew()
        {
            
        }

        public void OnRelease()
        {
            
        }
    }

    public class BundleNode
    {
        private string bundlePath;
        private AssetBundle assetBundle;
        private int refCount;

        private List<BundleNode> dependencies = new List<BundleNode>();

        public BundleNode() { }

        public void InitNode(string path,AssetBundle bundle)
        {
            bundlePath = path;
            assetBundle = bundle;
        }

        public void Retain()
        {
            ++refCount;
        }

        public void Release()
        {
            --refCount;
        }

        public UnityObject GetAsset(string assetPath)
        {
            return assetBundle.LoadAsset(assetPath);
        }

    }
}
