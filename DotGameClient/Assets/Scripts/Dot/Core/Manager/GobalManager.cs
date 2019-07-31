using Dot.Core.Event;
using Dot.Core.Entity;
using Dot.Core.Loader;
using Dot.Core.Pool;
using Dot.Core.Timer;
using Dot.Core.Util;
using System.Collections.Generic;

namespace Dot.Core.Manager
{
    public class GlobalManager : Singleton<GlobalManager>
    {
        private Dictionary<string, IGlobalManager> globalMgrs;
        private Dictionary<string, IGlobalUpdateManager> globalUpdateMgrs;

        public GlobalManager()
        {

        }

        private GameTimer timerMgr = null;
        public GameTimer TimerMgr
        {
            get
            {
                return timerMgr;
            }
        }

        private AssetManager assetMgr = null;
        public AssetManager AssetMgr
        {
            get
            {
                return assetMgr;
            }
        }

        protected override void DoInit()
        {
            globalMgrs = new Dictionary<string, IGlobalManager>();
            globalUpdateMgrs = new Dictionary<string, IGlobalUpdateManager>();

            //timerMgr = new GameTimer();
            //timerMgr.DoInit();
            //timerMgr.Priority = 0;
            //AddGlobalUpdateManager("TimerManager", timerMgr);

            //eventMgr = new EventManager();
            //eventMgr.DoInit();
            //eventMgr.Priority = 1;
            //AddGlobalManager("EventManager", eventMgr);

            assetMgr = new AssetManager(AssetType.Resources);
            assetMgr.DoInit();
            assetMgr.Priority = 3;

            //poolMgr = new PoolManager();
            //poolMgr.DoInit();
            //poolMgr.Priority = 4;
            //AddGlobalManager("PoolManager", poolMgr);

        }

        public override void DoDispose()
        {
            DontDestroyHandler.Destroy();

            List<IGlobalManager> mgrList = new List<IGlobalManager>();
            mgrList.AddRange(globalMgrs.Values);
            mgrList.AddRange(globalUpdateMgrs.Values);
            mgrList.Sort((x, y) =>
            {
                if(x.Priority>y.Priority)
                {
                    return 1;
                }else if(x.Priority<y.Priority)
                {
                    return -1;
                }else
                {
                    return 0;
                }
                
            });

            globalMgrs.Clear();
            globalUpdateMgrs.Clear();

            for(int i = mgrList.Count-1;i>=0;i--)
            {
                mgrList[i].DoDispose();
            }

            base.DoDispose();
        }

        public override void DoReset()
        {
            foreach (var kvp in globalMgrs)
            {
                kvp.Value.DoReset();
            }
            foreach(var kvp in globalUpdateMgrs)
            {
                kvp.Value.DoReset();
            }
        }

        public void DoUpdate(float deltaTime)
        {
            foreach (var kvp in globalUpdateMgrs)
            {
                kvp.Value.DoUpdate(deltaTime);
            }
        }

        public void AddGlobalManager<T>(T mgr) where T:IGlobalManager
        {
            AddGlobalManager(typeof(T).Name, mgr);
        }

        public void AddGlobalManager<T>(string mgrName,T mgr)where T:IGlobalManager
        {
            if (!globalMgrs.ContainsKey(mgrName))
            {
                globalMgrs.Add(mgrName, mgr);
            }
        }

        public void AddGlobalUpdateManager<T>(T mgr) where T: IGlobalUpdateManager
        {
            AddGlobalUpdateManager(typeof(T).Name, mgr);
        }

        public void AddGlobalUpdateManager<T>(string mgrName,T mgr)where T:IGlobalUpdateManager
        {
            if (!globalUpdateMgrs.ContainsKey(mgrName))
            {
                globalUpdateMgrs.Add(mgrName, mgr);
            }
        }

        public T GetManager<T>() where T:IGlobalManager
        {
            return GetManager<T>(typeof(T).Name);
        }

        public T GetManager<T>(string mgrName) where T : IGlobalManager
        {
            T mgr = default(T);
            if(globalMgrs.ContainsKey(mgrName))
            {
                mgr = (T)globalMgrs[mgrName];
            }else if(globalUpdateMgrs.ContainsKey(mgrName))
            {
                mgr = (T)globalUpdateMgrs[mgrName];
            }
            return mgr;
        }

        public object GetManager(string mgrName)
        {
            if(globalMgrs.ContainsKey(mgrName))
            {
                return globalMgrs[mgrName];
            }else if(globalUpdateMgrs.ContainsKey(mgrName))
            {
                return globalUpdateMgrs[mgrName];
            }
            return null;
        }
    }
}
