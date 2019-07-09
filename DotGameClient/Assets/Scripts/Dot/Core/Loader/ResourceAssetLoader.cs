using Dot.Core.Logger;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public class ResourceAsyncOperationData : AsyncOperationData
    {
        public ResourceAsyncOperationData(string path, AssetLoaderPriorityType priorityType) : base(path, priorityType)
        {
        }

        public override void CreateOperation()
        {
            operation = Resources.LoadAsync(path);
        }

        public override UnityObject GetAsset()
        {
            return (operation as ResourceRequest).asset;
        }
    }

    public class ResourceAssetLoader : AAssetLoader
    {
        private static readonly string ResourceAssetPrefix = "Assets/Resources/";

        public override void CancelAssetLoader(int index)
        {
            if(assetAsyncDataDic.TryGetValue(index,out AsyncData aData))
            {
                aData.Finish();
                assetAsyncDataDic.Remove(index);
            }
        }

        public override UnityObject LoadAsset(string assetFullPath)
        {
            string assetPath = GetAssetPath(assetFullPath);
            if(string.IsNullOrEmpty(assetPath))
            {
                return null;
            }
            return Resources.Load(assetPath);
        }

        public override int LoadAssetAsync(AssetAsyncData data)
        {
            int index = GetLoaderIndex();
            data.Index = index;

            assetAsyncDataDic.Add(index, data);
            string resourcePath = GetAssetPath(data.GetAssetFullPath());
            if(allAsyncOperationDic.TryGetValue(resourcePath, out AsyncOperationData aoData))
            {
                aoData.Retain();
                data.SetOperationData(0, aoData);
            }else
            {
                ResourceAsyncOperationData raoData = new ResourceAsyncOperationData(resourcePath, data.priority);
                allAsyncOperationDic.Add(resourcePath, raoData);

                data.SetOperationData(0, raoData);
                raoData.Retain();

                if(waitingOperationQueue.Count==waitingOperationQueue.MaxSize)
                {
                    waitingOperationQueue.Resize(waitingOperationQueue.MaxSize * 2);
                }
                waitingOperationQueue.Enqueue(raoData, raoData.Priority);
            }

            return index;
        }

        public override UnityObject[] LoadBatchAsset(string[] assetFullPaths)
        {
            UnityObject[] objs = new UnityObject[assetFullPaths.Length];
            for(int i =0;i<assetFullPaths.Length;i++)
            {
                objs[i] = LoadAsset(assetFullPaths[i]);
            }
            return objs;
        }

        public override int LoadBatchAssetAsync(BatchAssetAsyncData data)
        {
            int index = GetLoaderIndex();
            data.Index = index;

            assetAsyncDataDic.Add(index, data);

            string[] paths = data.GetBatchAssetFullPath();
            for(int i =0;i<paths.Length;i++)
            {
                string resourcePath = GetAssetPath(paths[i]);
                if (allAsyncOperationDic.TryGetValue(resourcePath, out AsyncOperationData aoData))
                {
                    aoData.Retain();
                    data.SetOperationData(i, aoData);
                }
                else
                {
                    ResourceAsyncOperationData raoData = new ResourceAsyncOperationData(resourcePath, data.priority);
                    allAsyncOperationDic.Add(resourcePath, raoData);

                    data.SetOperationData(i, raoData);
                    raoData.Retain();

                    if (waitingOperationQueue.Count == waitingOperationQueue.MaxSize)
                    {
                        waitingOperationQueue.Resize(waitingOperationQueue.MaxSize * 2);
                    }
                    waitingOperationQueue.Enqueue(raoData, raoData.Priority);
                }
            }
            return index;

        }

        public override void UnloadUnusedAsset()
        {
            Resources.UnloadUnusedAssets();
        }
        
        protected override string GetAssetPath(string assetFullPath)
        {
            if(assetFullPath.StartsWith(ResourceAssetPrefix))
            {
                string path = assetFullPath.Substring(ResourceAssetPrefix.Length);
                path = path.Substring(0, path.LastIndexOf('.'));
                return path;
            }else
            {
                DebugLogger.LogError("ResourceAssetLoader::GetAssetPath->");
            }
            return string.Empty;
        }
    }
}
