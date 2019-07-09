using Dot.Core;
using Dot.Core.Logger;
using System;
using System.IO;
using UnityEngine;
using XLua;

namespace Dot.XLuaEx
{
    [Serializable]
    public class LuaAsset
    {
        [SerializeField]
        private string m_ScriptName = "";
        [SerializeField]
        private string m_ScriptPath = "";

        public string LuaScriptPath
        {
            set
            {
                m_ScriptPath = value;
                if(!string.IsNullOrEmpty(m_ScriptPath))
                {
                    m_ScriptName = Path.GetFileNameWithoutExtension(m_ScriptPath);
                }else
                {
                    m_ScriptName = null;
                }
            }
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(m_ScriptName) || string.IsNullOrEmpty(m_ScriptPath);
        }

        public bool DoRequire()
        {
            if(string.IsNullOrEmpty(m_ScriptName) || string.IsNullOrEmpty(m_ScriptPath))
            {
                DebugLogger.LogError(string.Format("LuaAsset::DoRequire->ScriptName or ScriptPath is NULL!"));
                return false;
            }
            if(GameApplication.GLuaEnv.Global.ContainsKey<string>(m_ScriptName))
            {
                return true;
            }
            GameApplication.GLuaEnv.DoString(string.Format("require (\"{0}\")", m_ScriptPath));
            return true ;
        }

        public LuaTable DoRequireAndInstance()
        {
            if (string.IsNullOrEmpty(m_ScriptName) || string.IsNullOrEmpty(m_ScriptPath))
            {
                DebugLogger.LogError(string.Format("LuaAsset::GetInstance->scriptName is NULL!"));
                return null;
            }

            LuaTable classTable = GameApplication.GLuaEnv.Global.Get<LuaTable>(m_ScriptName);
            if (classTable == null)
            {
                GameApplication.GLuaEnv.DoString(string.Format("require (\"{0}\")", m_ScriptPath));
                classTable = GameApplication.GLuaEnv.Global.Get<LuaTable>(m_ScriptName);
            }
            if (classTable == null)
                return null;

            LuaFunction callFun = classTable.Get<LuaFunction>("__call");
            LuaTable objTable = callFun.Func<LuaTable, LuaTable>(classTable);
            callFun.Dispose();
            callFun = null;
            classTable.Dispose();
            classTable = null;

            return objTable;
        }
    }
}
