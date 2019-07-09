using Dot.Core.Logger;
using Dot.Core.Manager;
using Dot.Core.Timer;
using Dot.Core.Util;
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

        public bool isCull = false;
        public int cullOnceAmout = 0;
        public int cullDelay = 60;

        public int limitMaxAmount = 0;
        public int limitMinAmount = 0;

        private float preCullTime = 0;
        private float curTime = 0;

        private GameTimer timerMgr = null;
        private TimerTaskInfo preloadTimerTask = null;

        public UnityAction<string, string> onPreloadFinishCallBack = null; 

        internal GameObjectPool(SpawnPool pool, string aPath, GameObjectPoolItem item)
        {
            spawnPool = pool;
            assetPath = aPath;
            templateItem = item;

            timerMgr = GlobalManager.GetInstance().TimerMgr;
            preloadTimerTask = timerMgr.AddTimerTask(0.05f, 0, null, OnPreloadTimerUpdate, null, null);
        }

#if UNITY_EDITOR
        public GameObjectPoolItem[] GetItemsInPool()
        {
            List<GameObjectPoolItem> items = new List<GameObjectPoolItem>();
            items.AddRange(unusedItemQueue);

            for(int i=usedItemList.Count-1;i>=0;i--)
            {
                if(usedItemList[i].TryGetTarget(out GameObjectPoolItem item))
                {
                    items.Add(item);
                }else
                {
                    usedItemList.RemoveAt(i);
                }
            }
            return items.ToArray();
        }
#endif

        public GameObject GetGameObjectItem(bool isAutoSetActive = true)
        {
            GameObjectPoolItem pItem = GetPoolItem(isAutoSetActive);
            if(pItem!=null)
            {
                return pItem.CachedGameObject;
            }
            return null;
        }

        public GameObjectPoolItem GetPoolItem(bool isAutoSetActive = true)
        {
            if (limitMaxAmount != 0 && usedItemList.Count > limitMaxAmount)
            {
                DebugLogger.LogError("GameObjectPool::Get->Large than Max Amount");
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

        public void ReleaseGameObjectItem(GameObject item)
        {
            GameObjectPoolItem pItem = item.GetComponent<GameObjectPoolItem>();
            if(pItem == null)
            {
                DebugLogger.LogError("GameObjectPool::ReleaseGameObjectItem->poolItem is null");
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

        private void OnPreloadTimerUpdate(SystemObject obj)
        {
            int curAmount = usedItemList.Count + unusedItemQueue.Count;
            if(curAmount >= preloadTotalAmount)
            {
                timerMgr.RemoveTimerTask(preloadTimerTask);
                preloadTimerTask = null;
                onPreloadFinishCallBack(spawnPool.PoolName, assetPath);
                onPreloadFinishCallBack = null;
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
            if(curTime - preCullTime >= cullDelay)
            {
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
                for (int i = 0; i < cullAmout; i++)
                {
                    if (unusedItemQueue.Count > 0)
                    {
                        UnityObject.Destroy(unusedItemQueue.Dequeue().CachedGameObject);
                    } else
                    {
                        break;
                    }
                }

                preCullTime = curTime;
            }
        }

        

        internal bool ClearPool(bool isForce = false)
        {
            if(!isForce && isNeverClear)
            {
                return false;
            }

            if(preloadTimerTask!=null)
            {
                timerMgr.RemoveTimerTask(preloadTimerTask);
                preloadTimerTask = null;
            }
            timerMgr = null;

            for (int i = usedItemList.Count - 1; i >= 0; i--)
            {
                if(usedItemList[i].TryGetTarget(out GameObjectPoolItem item))
                {
                    UnityObject.Destroy(item.CachedGameObject);
                }
            }
            for (int i = unusedItemQueue.Count - 1; i >= 0; i--)
            {
                UnityObject.Destroy(unusedItemQueue.Dequeue().CachedGameObject);
            }
            unusedItemQueue.Clear();
            usedItemList.Clear();

            assetPath = null;
            spawnPool = null;
            templateItem = null;
            isNeverClear = false;

            return true;
        }

        private GameObjectPoolItem CreateNewItem()
        {
            GameObjectPoolItem item = GameObjectHandler.Instantiate(assetPath, templateItem);
            item.AssetPath = assetPath;
            item.PoolName = spawnPool.PoolName;

            item.CachedTransform.SetParent(spawnPool.CachedTransform, false);
            return item;
        }
    }
}
