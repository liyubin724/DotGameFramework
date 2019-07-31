using Dot.Core.Asset;
using Dot.Core.Logger;
using Dot.Core.Timer;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SystemObject = System.Object;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Pool
{
    public class GameObjectPool
    {
        private SpawnPool spawnPool = null;
        private string assetPath = null;
        private GameObjectPoolItem templateItem = null;

        private List<WeakReference<GameObjectPoolItem>> usedItemList = new List<WeakReference<GameObjectPoolItem>>();
        private Queue<GameObjectPoolItem> unusedItemQueue = new Queue<GameObjectPoolItem>();

        public bool isNeverClear = false;

        public int preloadTotalAmount = 0;
        public int preloadOnceAmout = 1;
        public UnityAction<string, string> preloadCompleteCallback = null; 

        public bool isCull = false;
        public int cullOnceAmout = 0;
        public int cullDelayTime = 30;

        public int limitMaxAmount = 0;
        public int limitMinAmount = 0;

        private float preCullTime = 0;
        private float curTime = 0;

        private TimerTaskInfo preloadTimerTask = null;

        internal GameObjectPool()
        {

        }

        internal void InitPool(SpawnPool pool, string aPath, GameObjectPoolItem item)
        {
            spawnPool = pool;
            assetPath = aPath;
            templateItem = item;
            templateItem.gameObject.SetActive(false);

            preloadTimerTask = GameApplication.GTimer.AddTimerTask(0.05f, 0, null, OnPreloadTimerUpdate, null, null);
        }

        #region Preload

        private void OnPreloadTimerUpdate(SystemObject obj)
        {
            int curAmount = usedItemList.Count + unusedItemQueue.Count;
            if (curAmount >= preloadTotalAmount)
            {
                PreloadComplete();
            }
            else
            {
                int poa = preloadOnceAmout;
                if (poa == 0)
                {
                    poa = preloadTotalAmount;
                }
                else
                {
                    poa = Mathf.Min(preloadOnceAmout, preloadTotalAmount - curAmount);
                }
                for (int i = 0; i < poa; ++i)
                {
                    GameObjectPoolItem item = CreateNewItem();
                    item.CachedGameObject.SetActive(false);
                    unusedItemQueue.Enqueue(item);
                }
            }
        }

        private void PreloadComplete()
        {
            preloadCompleteCallback?.Invoke(spawnPool.PoolName, assetPath);
            preloadCompleteCallback = null;

            if(preloadTimerTask!=null)
            {
                GameApplication.GTimer.RemoveTimerTask(preloadTimerTask);
                preloadTimerTask = null;
            }
        }

        #endregion

        #region GetItem
        public GameObject GetGameObjectItem(bool isAutoSetActive = true)
        {
            GameObjectPoolItem pItem = GetPoolItem(isAutoSetActive);
            if(pItem!=null)
            {
                return pItem.CachedGameObject;
            }
            return null;
        }

        public T GetComponentItem<T>(bool isAutoActive = true) where T:MonoBehaviour
        {
            GameObject gObj = GetGameObjectItem(isAutoActive);
            if(gObj!=null)
            {
                return gObj.GetComponent<T>();
            }
            return null;
        }

        public GameObjectPoolItem GetPoolItem(bool isAutoSetActive = true)
        {
            if (limitMaxAmount != 0 && usedItemList.Count > limitMaxAmount)
            {
                DebugLogger.LogWarning("GameObjectPool::Get->Large than Max Amount");
                return null;
            }

            GameObjectPoolItem item = null;
            if (unusedItemQueue.Count > 0)
            {
                item = unusedItemQueue.Dequeue();
            }else
            {
                item = CreateNewItem();
            }

            item.DoSpawned();

            if(isAutoSetActive)
            {
                item.CachedGameObject.SetActive(true);
            }
            usedItemList.Add(new WeakReference<GameObjectPoolItem>(item));
            return item;
        }

        #endregion

        #region Release Item
        public void ReleaseGameObjectItem(GameObject item)
        {
            GameObjectPoolItem pItem = item.GetComponent<GameObjectPoolItem>();
            if(pItem == null)
            {
                DebugLogger.LogWarning("GameObjectPool::ReleaseGameObjectItem->Can't found component which named GameObjectPoolItem.so it will be destroy");

                UnityObject.Destroy(item);

                return;
            }
            ReleasePoolItem(pItem);
        }

        public void ReleasePoolItem(GameObjectPoolItem item)
        {
            if(item == null)
            {
                DebugLogger.LogError("GameObjectPool::ReleaseItem->Item is Null");
                return;
            }

            item.DoDespawned();
            item.CachedTransform.SetParent(spawnPool.CachedTransform, false);
            item.CachedGameObject.SetActive(false);

            unusedItemQueue.Enqueue(item);
            for (int i = usedItemList.Count - 1; i >= 0; i--)
            {
                if (usedItemList[i].TryGetTarget(out GameObjectPoolItem usedItem))
                {
                    if(usedItem == item)
                    {
                        usedItemList.RemoveAt(i);
                        break;
                    }
                }
                else
                {
                    usedItemList.RemoveAt(i);
                }
            }
        }
        #endregion


        internal void CullPool(float deltaTime)
        {
            for (int i = usedItemList.Count - 1; i >= 0; i--)
            {
                if (!usedItemList[i].TryGetTarget(out GameObjectPoolItem usedItem))
                {
                    usedItemList.RemoveAt(i);
                }
            }

            if (!isCull)
            {
                return;
            }

            curTime += deltaTime;
            if(curTime - preCullTime < cullDelayTime)
            {
                return;
            }

            int cullAmout = 0;
            if (usedItemList.Count + unusedItemQueue.Count <= limitMinAmount)
            {
                cullAmout = 0;
            }
            else
            {
                cullAmout = usedItemList.Count + unusedItemQueue.Count - limitMinAmount;
                if (cullAmout > unusedItemQueue.Count)
                {
                    cullAmout = unusedItemQueue.Count;
                }
            }

            if (cullOnceAmout > 0 && cullOnceAmout < cullAmout)
            {
                cullAmout = cullOnceAmout;
            }

            for (int i = 0; i < cullAmout && unusedItemQueue.Count>0; i++)
            {
                UnityObject.Destroy(unusedItemQueue.Dequeue().CachedGameObject);
            }

            preCullTime = curTime;
        }
        
        internal bool ClearPool(bool isForce = false)
        {
            if(!isForce && isNeverClear)
            {
                return false;
            }

            preloadCompleteCallback = null;
            if (preloadTimerTask != null)
            {
                GameApplication.GTimer.RemoveTimerTask(preloadTimerTask);
                preloadTimerTask = null;
            }

            usedItemList.Clear();

            for (int i = unusedItemQueue.Count - 1; i >= 0; i--)
            {
                UnityObject.Destroy(unusedItemQueue.Dequeue().CachedGameObject);
            }
            unusedItemQueue.Clear();

            UnityObject.Destroy(templateItem);

            assetPath = null;
            spawnPool = null;
            templateItem = null;
            isNeverClear = false;

            return true;
        }

        private GameObjectPoolItem CreateNewItem()
        {
            GameObject gObj = AssetManager.GetInstance().Instantiate<GameObject>(assetPath);
            if(gObj!=null)
            {
                GameObjectPoolItem item = gObj.GetComponent<GameObjectPoolItem>();
                item.AssetPath = assetPath;
                item.PoolName = spawnPool.PoolName;

                item.CachedTransform.SetParent(spawnPool.CachedTransform, false);
                return item;
            }
            return null;
        }

#if UNITY_EDITOR
        public GameObjectPoolItem[] GetItemsInPool()
        {
            List<GameObjectPoolItem> items = new List<GameObjectPoolItem>();
            items.AddRange(unusedItemQueue);

            for (int i = usedItemList.Count - 1; i >= 0; i--)
            {
                if (usedItemList[i].TryGetTarget(out GameObjectPoolItem item))
                {
                    items.Add(item);
                }
                else
                {
                    usedItemList.RemoveAt(i);
                }
            }
            return items.ToArray();
        }
#endif

    }
}
