using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public class AssetDatabaseAsyncOperation : AAssetAsyncOperation
    {
        public AssetDatabaseAsyncOperation(string assetPath, string assetRoot) : base(assetPath, assetRoot)
        {
        }

        protected override void CreateAsyncOperation()
        {
            
        }

        public override void DoUpdate()
        {
            if(status == AssetAsyncOperationStatus.Loading)
            {
                status = AssetAsyncOperationStatus.Loaded;
            }
        }

        public override UnityObject GetAsset()
        {
            if(status == AssetAsyncOperationStatus.Loaded)
            {
#if UNITY_EDITOR
                return UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityObject));
#endif
            }
            return null;
        }

        public override float Progress()
        {
            if(status == AssetAsyncOperationStatus.Loaded)
            {
                return 1.0f;
            }
            return 0.0f;
        }
    }
}
