using Dot.Core.Entity.BaseEntity;
using Dot.Core.Event;
using Dot.Core.Logger;
using Dot.XLuaEx;
using XLua;

namespace Dot.Core.Entity.Controller
{
    public partial class EntityLuaController : EntityController
    {
        private LuaTable luaScriptTable = null;

        public EntityLuaController(EntityObject entityObj) : base(entityObj)
        {
        }

        public void BindLuaScript(string luaName)
        {
            luaScriptTable?.Dispose();
            luaScriptTable = null;

            if (string.IsNullOrEmpty(luaName))
            {
                return;
            }

            luaScriptTable = GameApplication.GLuaEnv.Global.Get<LuaTable>(luaName);
            if (luaScriptTable == null)
            {
                DebugLogger.LogError("");
            }
        }

        public void RequireAndBindLuaScript(string luaPath)
        {
            LuaAsset luaAsset = new LuaAsset();
            luaAsset.LuaScriptPath = luaPath;
            luaScriptTable = luaAsset.DoRequireAndInstance();
        }

        public void RegisterController(string name, EntityController controller)
        {
            luaScriptTable?.Set(name, controller);
        }

        public override void OnDispose()
        {
            luaScriptTable?.Dispose();
            luaScriptTable = null;
        }

        public override void OnReset()
        {
            base.OnReset();
        }

        protected override void AddEventListeners()
        {
            Entity.RegistEvent(EntityConst.EVENT_CALL_LUA_FUNCTION, CallLuaFunction);
        }

        protected override void RemoveEventListeners()
        {
            Entity.UnregisterEvent(EntityConst.EVENT_CALL_LUA_FUNCTION, CallLuaFunction);
        }

        private void CallLuaFunction(EventData eventData)
        {
            if(eventData.ParamCount<=0)
            {
                DebugLogger.LogError("");
                return;
            }

            string funName = eventData.GetValue<string>(0);
            if(string.IsNullOrEmpty(funName))
            {
                DebugLogger.LogError("");
                return;
            }
            LuaFunction function = luaScriptTable.Get<LuaFunction>(funName);
            if(function !=null)
            {
                function.Action(1, eventData.EventParams);

                function.Dispose();
                function = null;
            }else
            {
                DebugLogger.LogError("");
            }
        }
    }
}
