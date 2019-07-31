using UnityEngine;

namespace Dot.Core.Pool
{
    public class GameObjectPoolItem : MonoBehaviour
    {
        public string AssetPath { get; set; } = string.Empty;
        public string PoolName { get; set; } = string.Empty;

        private Transform cachedTransform = null;
        public Transform CachedTransform
        {
            get {
                if (cachedTransform == null)
                {
                    cachedTransform = transform;
                }
                return cachedTransform;
            }
        }

        private GameObject cachedGameObject = null;
        public GameObject CachedGameObject
        {
            get
            {
                if(cachedGameObject == null)
                {
                    cachedGameObject = gameObject;
                }
                return cachedGameObject;
            }
        } 

        public virtual void DoSpawned()
        {

        }

        public virtual void DoDespawned()
        {

        }

        public void ReleaseItem()
        {
            if(string.IsNullOrEmpty(AssetPath) || string.IsNullOrEmpty(PoolName))
            {
                Destroy(CachedGameObject);
            }
            SpawnPool spawnPool = PoolManager.GetInstance().GetSpawnPool(PoolName, false);
            if(spawnPool == null)
            {
                Destroy(CachedGameObject);
            }else
            {
                GameObjectPool gObjPool = spawnPool.GetGameObjectPool(AssetPath);
                if(gObjPool == null)
                {
                    Destroy(CachedGameObject);
                }else
                {
                    gObjPool.ReleasePoolItem(this);
                }
            }
        }
    }
}
