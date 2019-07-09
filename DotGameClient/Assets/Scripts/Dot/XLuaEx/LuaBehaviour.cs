using Dot.Core;
using Dot.Core.Logger;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace Dot.XLuaEx
{
    [Serializable]
    public class RegisterToLuaObject
    {
        [InfoBox("Name can be empty if it is a array")]
        public string name;

        [OnValueChanged("OnObjChanged")]
        public GameObject obj;

        [ReadOnly]
        public UnityEngine.Object regObj;

        [ValueDropdown("GetObjComponent")]
        [ShowIf("IsShowTypeName")]
        [OnValueChanged("OnTypeNameChanged")]
        public string typeName;

        #region Just for Odin

        private string[] allComponents = null;
        private void OnObjChanged()
        {
            allComponents = new string[0];
            regObj = null;
            
            if(obj!=null)
            {
                FindObjComponent();
                if (string.IsNullOrEmpty(typeName) || Array.IndexOf(allComponents,typeName)<0)
                {
                    typeName = "GameObject";
                }
                OnTypeNameChanged();
            }else
            {
                typeName = "";
            }
        }

        private void FindObjComponent()
        {
            if (obj != null)
            {
                List<string> compList = new List<string>();
                compList.Add("GameObject");
                MonoBehaviour[] monoBehaviours = obj.GetComponents<MonoBehaviour>();
                Array.ForEach(monoBehaviours, (m) =>
                {
                    compList.Add(m.GetType().Name);
                });
                allComponents = compList.ToArray();
            }
            else
            {
                allComponents = new string[0];
            }
        }

        private string[] GetObjComponent()
        {
            if(allComponents == null)
            {
                FindObjComponent();
            }
            return allComponents;
        }

        private bool IsShowTypeName()
        {
            return allComponents.Length > 0;
        }

        private void OnTypeNameChanged()
        {
            if(typeName == "GameObject")
            {
                regObj = obj;
            }else
            {
                regObj = obj.GetComponent(typeName);
            }
        }
        #endregion Just for odin
    }

    [Serializable]
    public class RegisterToLuaObjectArr
    {
        public string name;
        [ListDrawerSettings(Expanded = true)]
        public RegisterToLuaObject[] luaObjects;
    }

    [Serializable]
    public class RegisterToLuaBehaviour
    {
        public string name;
        public LuaBehaviour behaviour;
    }

    [Serializable]
    public class RegisterToLuaBehaviourArr
    {
        public string name;
        [ListDrawerSettings(Expanded =true)]
        public LuaBehaviour[] luaBehaviours;
    }

    public partial class LuaBehaviour : MonoBehaviour
    {
        [ValidateInput("ValidateLuaAssert",ContiniousValidationCheck =true,DefaultMessage ="需要指定正确的Lua脚本")]
        public LuaAsset luaAsset;

        [ValidateInput("ValidateRegLuaObject",DefaultMessage ="注册的Object有误，请检查。1 name不可为空且不可重复，2 obj与regObj不可为空")]
        public RegisterToLuaObject[] regLuaObject;
        [ValidateInput("ValidateregLuaObjectArr",DefaultMessage = "注册的Object有误，请检查.1 name不可为空且不可重复，2 子元素中obj与regObj不可为空")]
        public RegisterToLuaObjectArr[] regLuaObjectArr;
        [ValidateInput("ValidateRegLuaBehaviour",DefaultMessage = "注册的LuaBehaviour有误，请检查.1 name不可为空且不可重复，2 引用的behaviour不可为空")]
        public RegisterToLuaBehaviour[] regLuaBehaviour;
        [ValidateInput("ValidateRegLuaBehaviourArr", DefaultMessage = "注册的LuaBehaviour有误，请检查.1 name不可为空且不可重复，2 子元素引用的behaviour不可为空")]
        public RegisterToLuaBehaviourArr[] regLuaBehaviourArr;

        protected LuaEnv luaEnv;
        protected LuaTable objTable = null;

        public LuaTable ObjTable
        {
            get { return objTable; }
        }

        private bool isInited = false;
        public void InitLua()
        {
            if (isInited)
                return;

            isInited = true;

            luaEnv = GameApplication.GLua.GameLuaEnv;
            if (luaAsset != null && !luaAsset.IsEmpty())
            {
                objTable = luaAsset.DoRequireAndInstance();
                objTable.Set("gameObject", gameObject);
                objTable.Set("transform", transform);

                RegisterLuaObject();
                RegisterLuaObjectArr();

                RegisterLuaBehaviour();
                RegisterLuaBehaviourArr();
            }

        }

        protected virtual void Awake()
        {
            InitLua();
            objTable.Get<Action<LuaTable>>("DoAwake")?.Invoke(objTable);
        }

        void RegisterLuaBehaviour()
        {
            if (regLuaBehaviour != null && regLuaBehaviour.Length > 0)
            {
                for (int i = 0; i < regLuaBehaviour.Length; i++)
                {
                    if (regLuaBehaviour[i].behaviour == null)
                    {
                        DebugLogger.LogError("LuaBehaviour::RegisterLuaBehaviour->behaviour is null.objName = " + name + "  index = " + i);
                        continue;
                    }
                    regLuaBehaviour[i].behaviour.InitLua();

                    objTable.Set<string, LuaTable>(regLuaBehaviour[i].name, regLuaBehaviour[i].behaviour.ObjTable);
                }
            }
        }

        void RegisterLuaBehaviourArr()
        {
            if (regLuaBehaviourArr != null && regLuaBehaviourArr.Length > 0)
            {
                for (int i = 0; i < regLuaBehaviourArr.Length; i++)
                {
                    if (string.IsNullOrEmpty(regLuaBehaviourArr[i].name))
                    {
                        DebugLogger.LogError("LuaBehaviour::RegisterLuaBehaviourArr->Group Name is Null, index = " + i);
                        continue;
                    }

                    LuaTable behTable = luaEnv.NewTable();

                    LuaBehaviour[] behs = regLuaBehaviourArr[i].luaBehaviours;
                    if (behs != null && behs.Length > 0)
                    {
                        for (int j = 0; j < behs.Length; j++)
                        {
                            if (behs[j] == null)
                            {
                                DebugLogger.LogError("LuaBehaviour::RegisterLuaBehaviourArr->behaviour is Null, index = " + j);
                                continue;
                            }
                            behs[j].InitLua();
                            behTable.Set<int, LuaTable>(j + 1, behs[j].ObjTable);

                        }
                    }

                    objTable.Set<string, LuaTable>(regLuaBehaviourArr[i].name, behTable);
                    behTable.Dispose();
                    behTable = null;
                }
            }
        }

        void RegisterLuaObject()
        {
            for (int i = 0; i < regLuaObject.Length; i++)
            {
                if (regLuaObject[i].obj == null || regLuaObject[i].regObj == null)
                {
                    DebugLogger.LogError("LuaBehaviour::RegisterLuaObjects->obj or regObj is Null");
                    continue;
                }
                string regName = regLuaObject[i].name;
                if (string.IsNullOrEmpty(regName))
                {
                    regName = regLuaObject[i].regObj.name;
                }

                objTable.Set(regName, regLuaObject[i].regObj);
            }
        }

        void RegisterLuaObjectArr()
        {
            if (regLuaObjectArr != null && regLuaObjectArr.Length > 0)
            {
                for (int i = 0; i < regLuaObjectArr.Length; i++)
                {
                    if (string.IsNullOrEmpty(regLuaObjectArr[i].name))
                    {
                        DebugLogger.LogError("LuaBehaviour::RegisterLuaObjectArr->Group Name is Null, index = " + i);
                        continue;
                    }

                    LuaTable regTable = luaEnv.NewTable();

                    RegisterToLuaObject[] luaObjs = regLuaObjectArr[i].luaObjects;
                    if (luaObjs != null && luaObjs.Length > 0)
                    {
                        for (int j = 0; j < luaObjs.Length; j++)
                        {
                            if (luaObjs[j].regObj == null)
                            {
                                DebugLogger.LogError("LuaBehaviour::RegisterLuaObjectArr->obj or regObj is Null");
                                continue;
                            }

                            regTable.Set(j + 1, luaObjs[j].regObj);
                        }
                    }
                    objTable.Set<string, LuaTable>(regLuaObjectArr[i].name, regTable);
                    regTable.Dispose();
                    regTable = null;
                }
            }
        }

        void Start()
        {
            if (objTable != null)
            {
                objTable.Get<Action<LuaTable>>("DoStart")?.Invoke(objTable);
            }
        }

        void OnEnable()
        {
            if (objTable != null)
            {
                objTable.Get<Action<LuaTable>>("DoEnable")?.Invoke(objTable);
            }
        }

        void OnDisable()
        {
            if (objTable != null)
            {
                objTable.Get<Action<LuaTable>>("DoDisable")?.Invoke(objTable);
            }
        }

        void OnDestroy()
        {
            if (luaEnv == null || objTable == null)
            {
                return;
            }
            objTable.Get<Action<LuaTable>>("DoDestroy")?.Invoke(objTable);
            objTable.Dispose();
            objTable = null;
        }


        #region Just for Odin
        private static bool ValidateLuaAssert(LuaAsset luaAsset)
        {
            if (luaAsset == null)
            {
                return false;
            }
            return !luaAsset.IsEmpty();
        }

        private static bool ValidateRegLuaObject(RegisterToLuaObject[] regLuaObject)
        {
            if (regLuaObject == null || regLuaObject.Length == 0)
            {
                return true;
            }
            List<string> names = new List<string>();
            foreach (var v in regLuaObject)
            {
                if (v.obj == null || v.regObj == null)
                {
                    return false;
                }
                if (string.IsNullOrEmpty(v.name))
                {
                    return false;
                }
                if (names.IndexOf(v.name) >= 0)
                {
                    return false;
                }
                names.Add(v.name);
            }
            return true;
        }

        private static bool ValidateregLuaObjectArr(RegisterToLuaObjectArr[] regLuaObjectArr)
        {
            if (regLuaObjectArr == null || regLuaObjectArr.Length == 0)
                return true;

            List<string> names = new List<string>();
            foreach (var v in regLuaObjectArr)
            {
                if (string.IsNullOrEmpty(v.name))
                {
                    return false;
                }
                if (names.IndexOf(v.name) >= 0)
                {
                    return false;
                }
                names.Add(v.name);
                foreach (var c in v.luaObjects)
                {
                    if (c.regObj == null || c.obj == null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool ValidateRegLuaBehaviour(RegisterToLuaBehaviour[] regLuaBehaviour)
        {
            if (regLuaBehaviour == null || regLuaBehaviour.Length == 0)
            {
                return true;
            }
            List<string> names = new List<string>();
            foreach (var v in regLuaBehaviour)
            {
                if (string.IsNullOrEmpty(v.name) || v.behaviour == null)
                {
                    return false;
                }
                if (names.IndexOf(v.name) >= 0)
                {
                    return false;
                }
                names.Add(v.name);
            }
            return true;
        }

        private static bool ValidateRegLuaBehaviourArr(RegisterToLuaBehaviourArr[] regLuaBehaviourArr)
        {
            if (regLuaBehaviourArr == null || regLuaBehaviourArr.Length == 0)
            {
                return true;
            }
            List<string> names = new List<string>();
            foreach (var v in regLuaBehaviourArr)
            {
                if (string.IsNullOrEmpty(v.name))
                {
                    return false;
                }
                if (names.IndexOf(v.name) >= 0)
                {
                    return false;
                }
                names.Add(v.name);
                foreach (var c in v.luaBehaviours)
                {
                    if (c == null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        #endregion
    }
}
