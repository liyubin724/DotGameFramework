using LuaAPI = XLua.LuaDLL.Lua;
using SystemObject = System.Object;

namespace XLua
{
    public partial class LuaFunction : LuaBase
    {
        public void Action()
        {
#if THREAD_SAFE || HOTFIX_ENABLE
            lock (luaEnv.luaEnvLock)
            {
#endif
            var L = luaEnv.L;
            int oldTop = LuaAPI.lua_gettop(L);
            int errFunc = LuaAPI.load_error_func(L, luaEnv.errorFuncRef);
            LuaAPI.lua_getref(L, luaReference);
            int error = LuaAPI.lua_pcall(L, 0, 0, errFunc);
            if (error != 0)
                luaEnv.ThrowExceptionFromError(oldTop);
            LuaAPI.lua_settop(L, oldTop);
#if THREAD_SAFE || HOTFIX_ENABLE
            }
#endif
        }

        public void Action(int startIndex = 0,params SystemObject[] objs)
        {
#if THREAD_SAFE || HOTFIX_ENABLE
            lock (luaEnv.luaEnvLock)
            {
#endif
            var L = luaEnv.L;
            var translator = luaEnv.translator;
            int oldTop = LuaAPI.lua_gettop(L);
            int errFunc = LuaAPI.load_error_func(L, luaEnv.errorFuncRef);
            LuaAPI.lua_getref(L, luaReference);
            int argsCount = 0;
            if (objs != null && objs.Length > startIndex)
            {
                for (int i = startIndex; i < objs.Length; i++)
                {
                    translator.PushByType(L, objs[i]);
                }
                argsCount = objs.Length - startIndex;
            }
            int error = LuaAPI.lua_pcall(L, argsCount, 0, errFunc);
            if (error != 0)
                luaEnv.ThrowExceptionFromError(oldTop);
            LuaAPI.lua_settop(L, oldTop);

#if THREAD_SAFE || HOTFIX_ENABLE
            }
#endif
        }
    }
}
