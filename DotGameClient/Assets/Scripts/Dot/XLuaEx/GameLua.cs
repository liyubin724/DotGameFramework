using Dot.Core;
using Dot.Core.Logger;
using Dot.Core.Timer;
using Dot.XLuaEx;
using System;
using XLua;

namespace Dot.XLuaEx
{
    public class GameLua
    {
        private LuaEnv luaEnv = null;
        public LuaEnv GameLuaEnv
        {
            get
            {
                return luaEnv;
            }
        }
        private TimerTaskInfo tickTaskInfo = null;

        public float TickInterval { get; set; } = 60;

        public GameLua()
        {
        }

        private bool isInit = false;
        public void InitLuaEnv(LuaAsset[] requireLuaAsset)
        {
            if(isInit)
            {
                return;
            }
            isInit = true;
            luaEnv = new LuaEnv();
#if DEBUG
            luaEnv.Global.Set<string, bool>("IsDebug", true);
#endif
            string[] scriptPath = new string[]
            {
                LuaConfig.LuaPathFormat,
            };
            LuaScriptLoader.SetScriptLoadFromFile(luaEnv, scriptPath);

            if (requireLuaAsset != null && requireLuaAsset.Length > 0)
            {
                for (int i = 0; i < requireLuaAsset.Length; i++)
                {
                    RequireLuaAsset(requireLuaAsset[i]);
                }
            }
        }

        public void RequireLuaAsset(LuaAsset luaAsset)
        {
            if(luaAsset!=null)
            {
                luaAsset.DoRequire();
            }
        }

        public void FullGC()
        {
            luaEnv.FullGc();
        }
        
        private bool isStarted = false;
        private string luaMgrName = "LuaManager";
        private Action<float> luaMgrUpdateAction = null;
        private Action luaMgrEndAction = null;
        private Action luaMgrResetAction = null;
        private Action luaMgrDestroyAction = null;
        
        public void StartLuaEnv(string luaMgrName = "LuaManager")
        {
            if (!isInit)
            {
                DebugLogger.LogError("");
                return;
            }
            if(isStarted)
            {
                DebugLogger.LogError("");
                return;
            }
            if(luaEnv == null)
            {
                return;
            }


            isStarted = true;
            this.luaMgrName = luaMgrName;
            if (TickInterval > 0 && luaEnv!=null)
            {
                tickTaskInfo = GameApplication.GTimer.AddTimerTask(TickInterval, 0, null, (obj) =>
                {
                    luaEnv.Tick();
                }, null, null);
            }

            if(!string.IsNullOrEmpty(luaMgrName))
            {
                LuaTable luaMgrTable = luaEnv.Global.Get<LuaTable>(luaMgrName);
                if(luaMgrTable!=null)
                {
                    Action<LuaTable> luaMgrStartAction = luaMgrTable.Get<Action<LuaTable>>("DoStart");
                    luaMgrStartAction?.Invoke(luaMgrTable);
                    luaMgrStartAction = null;

                    luaMgrUpdateAction = luaMgrTable.Get<Action<float>>("DoUpdate");
                    luaMgrDestroyAction = luaMgrTable.Get<Action>("DoDestroy");
                    luaMgrResetAction = luaMgrTable.Get<Action>("DoReset");
                    luaMgrResetAction = luaMgrTable.Get<Action>("DoEnd");

                    
                    luaMgrTable.Dispose();
                }else
                {
                    DebugLogger.LogError("");
                }
            }
        }

        public void EndLuaEnv()
        {
            if(isStarted)
            {
                isStarted = false;

                luaMgrEndAction?.Invoke();
                luaMgrEndAction = null;
                luaMgrUpdateAction = null;
                luaMgrResetAction = null;
                luaMgrDestroyAction = null;

                if(tickTaskInfo!=null)
                {
                    GameApplication.GTimer.RemoveTimerTask(tickTaskInfo);
                }
            }
            if(isInit)
            {
                DoDispose();
            }
        }

        public void DoUpdate(float deltaTime)
        {
            luaMgrUpdateAction?.Invoke(deltaTime);
        }

        public void DoRest()
        {
            luaMgrResetAction?.Invoke();
        }
        
        public void DoDispose()
        {
            luaMgrDestroyAction?.Invoke();
            if(luaEnv!=null)
            {
                luaEnv.Dispose();
                luaEnv = null;
            }
        }
    }
}
