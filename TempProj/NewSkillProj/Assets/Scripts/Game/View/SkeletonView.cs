using UnityEngine;

public class SkeletonView : VirtualView, ISkeletonView
{
    protected GameObject skeletonGameObject;

    public SkeletonView(string name) : base(name)
    {
    }

    public SkeletonView(string name, Transform parent) : base(name, parent)
    {

    }

    public void AddSkeleton(GameObject go)
    {
        if(skeletonGameObject!=null)
        {
            RemoveSkeleton();
        }
        skeletonGameObject = go;
        skeletonGameObject.transform.SetParent(RootTransform, false);
    }

    public void RemoveSkeleton()
    {
        if (skeletonGameObject != null)
        {
            Object.Destroy(skeletonGameObject);
        }
        skeletonGameObject = null;
    }

}
