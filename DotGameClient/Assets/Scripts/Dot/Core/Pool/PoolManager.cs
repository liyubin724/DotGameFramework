using Dot.Core.Logger;
using Dot.Core.Manager;
using Dot.Core.Timer;
using Dot.Core.Util;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Pool
{
    public class PoolManager : Singleton<PoolManager>
    {
        private Transform cachedTransform = null;
        private Dictionary<string, SpawnPool> spawnDic = new Dictionary<string, SpawnPool>();

        private float cullTimeInterval = 30f;
        private TimerTaskInfo cullTimerTask = null;

        protected override void DoInit()
        {
            cachedTransform = DontDestroyHandler.CreateTransform("PoolManager");
            cullTimerTask = GameApplication.GTimer.AddTimerTask(cullTimeInterval, 0, null, OnCullTimerUpdate, null, null);
        }
        
        public SpawnPool GetSpawnPool(string name, bool createIfNot = true)
        {
            if (spawnDic.TryGetValue(name, out SpawnPool pool))
            {
                return pool;
            }
            else
            {
                if (createIfNot)
                {
                    pool = new SpawnPool();
                    pool.InitSpawn(name, cachedTransform);

                    spawnDic.Add(name, pool);
                }
            }
            return pool;
        }

        public void ReleaseGameObjectToPool(GameObject go)
        {
            GameObjectPoolItem pItem = go.GetComponent<GameObjectPoolItem>();
            if (pItem == null)
            {
                DebugLogger.LogError("PoolManager::ReleaseGameObjectToPool->poolItem is null");

                UnityObject.Destroy(go);
                return;
            }
            pItem.ReleaseItem();
        }

        private void OnCullTimerUpdate(System.Object obj)
        {
            foreach(var kvp in spawnDic)
            {
                kvp.Value.CullSpawn(cullTimeInterval);
            }
        }

        public override void DoReset()
        {
            foreach (var kvp in spawnDic)
            {
                kvp.Value.ClearPool(true);
            }
            spawnDic.Clear();
        }

        public override void DoDispose()
        {
            DoReset();
            if(cullTimerTask != null)
            {
                GameApplication.GTimer.RemoveTimerTask(cullTimerTask);
            }
            cullTimerTask = null;
            spawnDic = null;
        }


#if UNITY_EDITOR
        public string[] GetSpawnPoolNames()
        {
            string[] names = new string[spawnDic.Count];
            int i = 0;
            foreach (var key in spawnDic.Keys)
            {
                names[i] = key;
                ++i;
            }

            System.Array.Sort(names);
            return names;
        }
#endif
    }
}
