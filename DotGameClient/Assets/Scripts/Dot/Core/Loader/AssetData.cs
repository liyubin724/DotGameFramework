using Priority_Queue;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public enum AsyncOperationState
    {
        None = 0,
        Waiting,
        Loading,
        Done,
        Error,
    }

    public abstract class AsyncOperationData : FastPriorityQueueNode
    {
        protected string path;
        protected AsyncOperationState state = AsyncOperationState.None;
        protected AsyncOperation operation = null;
        protected int retainCount = 0;

        public float RetainCount { get => retainCount; }

        public float Progress
        {
            get
            {
                if (state == AsyncOperationState.None || state == AsyncOperationState.Waiting)
                {
                    return 0f;
                }
                else if (state == AsyncOperationState.Loading)
                {
                    return operation.progress;
                }
                else if (state == AsyncOperationState.Done || state == AsyncOperationState.Error)
                {
                    return 1f;
                }
                return 0f;
            }
        }

        public bool IsDone
        {
            get
            {
                return state == AsyncOperationState.Done || state == AsyncOperationState.Error;
            }
        }

        public AsyncOperationData(string path, AssetLoaderPriorityType priorityType)
        {
            this.path = path;
            Priority = (float)priorityType;
            state = AsyncOperationState.Waiting;
        }


        public void Retain()
        {
            ++retainCount;
        }

        public void Release()
        {
            --retainCount;
        }

        public void StartLoad()
        {
            state = AsyncOperationState.Loading;
            CreateOperation();
        }

        public abstract void CreateOperation();

        public abstract UnityObject GetAsset();

        public void DoUpdate()
        {
            if (state == AsyncOperationState.Loading)
            {
                if (operation.isDone)
                {
                    state = AsyncOperationState.Done;
                    if (GetAsset() == null)
                    {
                        state = AsyncOperationState.Error;
                    }
                }
            }
        }
    }

    public abstract class AsyncData
    {
        protected int index;
        protected string[] fullPaths;
        protected AsyncOperationData[] operations;
        public AssetLoaderPriorityType priority = AssetLoaderPriorityType.Default;

        public int Index { get; set; }

        public AsyncData(string[] paths,AssetLoaderPriorityType priorityType)
        {
            fullPaths = paths;
            operations = new AsyncOperationData[fullPaths.Length];
            priority = priorityType;
        }
        
        public void SetOperationData(int pos,AsyncOperationData data)
        {
            operations[pos] = data;
        }

        public abstract string GetAssetFullPath();
        public abstract string[] GetBatchAssetFullPath();

        protected abstract void InvokeFinishCallback();
        protected abstract void InvokeProgressCallback();

        public float GetProgress()
        {
            if(operations!=null)
            {
                float progress = 0.0f;
                foreach(var data in operations)
                {
                    progress += data.Progress;
                }
                return progress;
            }
            return 0f;
        }

        public bool UpdateOperation()
        {
            bool isDone = true;
            foreach(var oper in operations)
            {
                oper.DoUpdate();
                if(isDone && !oper.IsDone)
                {
                    isDone = false;
                }
            }
            if(isDone)
            {
                InvokeFinishCallback();
            }else
            {
                InvokeProgressCallback();
            }
            return isDone;
        }

        public virtual void Finish()
        {
            foreach(var oper in operations)
            {
                oper.Release();
            }
            operations = null;
        }
    }

    public class AssetAsyncData : AsyncData
    {
        protected OnAssetLoadFinish finishCallback;
        protected OnAssetLoadProgress progressCallback = null;

        public AssetAsyncData(string path, 
            OnAssetLoadFinish finish,OnAssetLoadProgress progress, 
            AssetLoaderPriorityType priorityType):base(new string[1] { path},priorityType)
        {
            finishCallback = finish;
            progressCallback = progress;
        }

        public override string GetAssetFullPath()
        {
            if(fullPaths!=null && fullPaths.Length>0)
            {
                return fullPaths[0];
            }
            return string.Empty;
        }

        public override string[] GetBatchAssetFullPath()
        {
            throw new System.NotImplementedException();
        }

        protected override void InvokeFinishCallback()
        {
            if(finishCallback == null)
            {
                return;
            }
            UnityObject obj = null;
            if(operations!=null && operations[0]!=null)
            {
                obj = operations[0].GetAsset();
            }
            finishCallback(index, fullPaths[0], obj);
        }

        protected override void InvokeProgressCallback()
        {
            if(progressCallback == null)
            {
                return;
            }
            float progress = 0.0f;
            if (operations != null && operations[0] != null)
            {
                progress = operations[0].Progress;
            }
            progressCallback(index, fullPaths[0], progress);
        }

        public override void Finish()
        {
            base.Finish();
            progressCallback = null;
            finishCallback = null;
        }
    }

    public class BatchAssetAsyncData : AsyncData
    {
        public OnBatchAssetLoadFinish finishCallback;
        public OnBatchAssetLoadProgress progressCallback = null;

        public BatchAssetAsyncData(string[] paths, 
            OnBatchAssetLoadFinish finish,OnBatchAssetLoadProgress progress, 
            AssetLoaderPriorityType priorityType) : base(paths, priorityType)
        {
            finishCallback = finish;
            progressCallback = progress;
        }

        public override string GetAssetFullPath()
        {
            throw new System.NotImplementedException();
        }

        public override string[] GetBatchAssetFullPath()
        {
            return fullPaths;
        }

        protected override void InvokeFinishCallback()
        {
            if (finishCallback == null)
            {
                return;
            }
            UnityObject[] objs = new UnityObject[fullPaths.Length];
            for(int i =0;i<fullPaths.Length;i++)
            {
                if(operations == null || operations[i] == null)
                {
                    objs[i] = null;
                }else
                {
                    objs[i] = operations[i].GetAsset();
                }
            }
            finishCallback(index, fullPaths, objs);
        }

        protected override void InvokeProgressCallback()
        {
            if (progressCallback == null)
            {
                return;
            }
            float[] progresses = new float[fullPaths.Length];
            for(int i =0;i<fullPaths.Length;i++)
            {
                if (operations == null || operations[i] == null)
                {
                    progresses[i] = 0.0f;
                }
                else
                {
                    progresses[i] = operations[i].Progress;
                }
            }
            progressCallback(index, fullPaths, progresses);
        }

        public override void Finish()
        {
            base.Finish();
            finishCallback = null;
            progressCallback = null;
        }
    }

    


}
