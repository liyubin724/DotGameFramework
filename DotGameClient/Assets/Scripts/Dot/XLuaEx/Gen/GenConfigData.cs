//using Sirenix.OdinInspector;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//namespace Dot.XLuaEx.Gen
//{
//    public class GenConfigData : ScriptableObject
//    {
//        [InfoBox("需要处理的DLL的名称")]
//        [OnValueChanged("OnDllNameChanged", true)]
//        [ListDrawerSettings(ShowIndexLabels = true, DraggableItems = false, Expanded = false, ShowItemCount = true)]
//        public List<string> genDllNameList = new List<string>();

//        [InfoBox("需要忽略的命名空间")]
//        [OnValueChanged("OnExcludeNamespaceChanged")]
//        [ListDrawerSettings(ShowIndexLabels = true, DraggableItems = false, Expanded = false, ShowItemCount = true)]
//        public List<string> excludeNamespaceList = new List<string>();

//        [InfoBox("一个C#纯值类型或者C#枚举值加上了这个配置。xLua会为该类型生成gc优化代码")]
//        [ValueDropdown("GetAllValueTypeFullName", IsUniqueList = true)]
//        public List<string> gcOptimizeTypeNameList = new List<string>();

//        [InfoBox("一个C#类型加了这个配置，xLua会生成这个类型的适配代码（包括构造该类型实例，访问其成员属性、方法，静态属性、方法），" +
//            "否则将会尝试用性能较低的反射方式来访问")]
//        [ValueDropdown("GetAllTypeFullName", IsUniqueList = true)]
//        public List<string> luaCallCSharpTypeNameList = new List<string>();

//        [InfoBox("如果希望把一个lua函数适配到一个C# delegate,或者把一个lua table适配到一个C# interface，该delegate或者interface需要加上该配置。")]
//        [ValueDropdown("GetAllTypeFullName", IsUniqueList = true)]
//        public List<string> csharpCallLuaTypeNameList = new List<string>();

//        [InfoBox("如果你不要生成一个类型的一些成员的适配代码，你可以通过这个配置来实现。")]
//        [ListDrawerSettings(ShowIndexLabels = false, DraggableItems = false, Expanded = false, ShowItemCount = true, CustomAddFunction = "AddNewBlackData")]
//        public List<GenBlackData> blackTypeInfoList = new List<GenBlackData>();


//        private void AddNewBlackData()
//        {
//            blackTypeInfoList.Insert(0, new GenBlackData());
//        }
//        [Button(Name = "Auto Add BlackData")]
//        private void AutoAddBlackData()
//        {
//            for (int i = blackTypeInfoList.Count - 1; i >= 0; i--)
//            {
//                if (blackTypeInfoList[i].isAutoFound)
//                {
//                    blackTypeInfoList.RemoveAt(i);
//                }
//            }

//            List<Type> allTypes = new List<Type>();
//            allTypes.AddRange(GenConfig.GetTypes(luaCallCSharpTypeNameList));
//            allTypes.AddRange(GenConfig.GetTypes(csharpCallLuaTypeNameList));

//            foreach (var t in allTypes)
//            {
//                PropertyInfo[] pInfos = t.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
//                foreach (var p in pInfos)
//                {
//                    if (IsAutoBlack(p))
//                    {
//                        GenBlackData data = new GenBlackData()
//                        {
//                            isAutoFound = true,
//                            typeName = t.FullName,
//                            memberType = GenBlackMemberType.Property,
//                            memberInfoName = p.Name,
//                        };
//                        data.OnTypeChanged();
//                        blackTypeInfoList.Add(data);
//                    }
//                }

//                FieldInfo[] fInfos = t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
//                foreach (var f in fInfos)
//                {
//                    if (IsAutoBlack(f))
//                    {
//                        GenBlackData data = new GenBlackData()
//                        {
//                            isAutoFound = true,
//                            typeName = t.FullName,
//                            memberType = GenBlackMemberType.Field,
//                            memberInfoName = f.Name,
//                        };
//                        data.OnTypeChanged();
//                        blackTypeInfoList.Add(data);
//                    }
//                }

//                MethodInfo[] mInfos = t.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
//                foreach (var m in mInfos)
//                {
//                    if (IsAutoBlack(m))
//                    {
//                        GenBlackData data = new GenBlackData()
//                        {
//                            isAutoFound = true,
//                            typeName = t.FullName,
//                            memberType = GenBlackMemberType.Method,
//                            memberInfoName = m.Name,

//                        };
//                        Array.ForEach<ParameterInfo>(m.GetParameters(), (p) =>
//                        {
//                            data.methodParamJoinName += p.ParameterType.FullName + ",";
//                        });
//                        data.OnTypeChanged();
//                        blackTypeInfoList.Add(data);
//                    }
//                }

//            }

//        }

//        private bool IsAutoBlack(MemberInfo mInfo)
//        {
//            Attribute attr = mInfo.GetCustomAttribute<NoGenAttribute>();
//            if (attr != null)
//            {
//                return true;
//            }
//            return false;
//        }

//        private void OnDllNameChanged()
//        {
//            GenConfig.FilterDll();
//        }

//        private void OnExcludeNamespaceChanged()
//        {
//            GenConfig.FilterDll();
//        }

//        private string[] GetAllValueTypeFullName()
//        {
//            return GenConfig.AllValueTypeNames;
//        }

//        private string[] GetAllTypeFullName()
//        {
//            return GenConfig.AllClassTypeNames;
//        }
//    }

//    public enum GenBlackMemberType
//    {
//        Field,
//        Property,
//        Method,
//    }

//    [Serializable]
//    public class GenBlackData
//    {
//        [OnValueChanged("OnTypeChanged")]
//        [ValueDropdown("GetAllTypeFullName", IsUniqueList = true)]
//        public string typeName;

//        [EnumToggleButtons]
//        [OnValueChanged("OnTypeChanged")]
//        public GenBlackMemberType memberType = GenBlackMemberType.Field;

//        [ValueDropdown("GetMemberInfos")]
//        [OnValueChanged("OnMemberInfoChanged")]
//        [ShowIf("IsShowMemberInfoName")]
//        public string memberInfoName;

//        [ValueDropdown("GetMethodParamJoinName")]
//        [ShowIf("IsShowMethodParamJoinName")]
//        public string methodParamJoinName = "";

//        [ReadOnly]
//        public bool isAutoFound = false;

//        private string[] GetAllTypeFullName()
//        {
//            return GenConfig.AllClassTypeNames;
//        }

//        private string[] memberInfos = new string[0];
//        internal void OnTypeChanged()
//        {
//            memberInfos = new string[0];
//            if (!string.IsNullOrEmpty(typeName))
//            {
//                FindMemeberInfos();
//                if (memberInfos.Length > 0)
//                {
//                    if (Array.IndexOf(memberInfos, memberInfoName) < 0)
//                    {
//                        memberInfoName = memberInfos[0];
//                    }
//                    OnMemberInfoChanged();
//                }
//                else
//                {
//                    memberInfoName = "";
//                    methodParamJoinName = "";
//                    methodParamJoinNames = new string[0];
//                }
//            }
//        }

//        internal string[] GetMemberInfos()
//        {
//            return memberInfos;
//        }

//        private bool IsShowMethodParamJoinName()
//        {
//            return memberType == GenBlackMemberType.Method;
//        }

//        private bool IsShowMemberInfoName()
//        {
//            return memberInfos.Length > 0;
//        }

//        private void FindMemeberInfos()
//        {
//            List<Type> types = GenConfig.GetTypes(new List<string>() { typeName });
//            if (types != null && types.Count > 0)
//            {
//                Type type = types[0];
//                if (memberType == GenBlackMemberType.Field)
//                {
//                    FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
//                    List<string> fields = new List<string>();
//                    foreach (var fi in fieldInfos)
//                    {
//                        string fn = fi.Name;
//                        if (!fields.Contains(fn))
//                        {
//                            fields.Add(fn);
//                        }
//                    }
//                    memberInfos = fields.ToArray();
//                }
//                else if (memberType == GenBlackMemberType.Property)
//                {
//                    PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
//                    List<string> properties = new List<string>();
//                    foreach (var pi in propertyInfos)
//                    {
//                        string pn = pi.Name;
//                        if (!properties.Contains(pn))
//                        {
//                            properties.Add(pn);
//                        }
//                    }
//                    memberInfos = properties.ToArray();
//                }
//                else if (memberType == GenBlackMemberType.Method)
//                {
//                    MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
//                    List<string> methods = new List<string>();
//                    foreach (var mi in methodInfos)
//                    {
//                        string mn = mi.Name;
//                        if (!methods.Contains(mn) && !mn.StartsWith("get_") && !mn.StartsWith("set_"))
//                        {
//                            methods.Add(mn);
//                        }

//                    }
//                    memberInfos = methods.ToArray();
//                }
//            }
//        }

//        private string[] methodParamJoinNames = new string[0];
//        internal void OnMemberInfoChanged()
//        {
//            methodParamJoinNames = new string[0];
//            if (!string.IsNullOrEmpty(memberInfoName) && memberType == GenBlackMemberType.Method)
//            {
//                List<Type> types = null;//GenConfig.GetTypes(new List<string>() { typeName });
//                if (types != null && types.Count > 0)
//                {
//                    Type type = types[0];

//                    MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
//                    List<string> mps = new List<string>();
//                    foreach (var mi in methodInfos)
//                    {
//                        string mn = mi.Name;
//                        if (mn == memberInfoName)
//                        {
//                            StringBuilder sb = new StringBuilder();
//                            ParameterInfo[] parameterInfos = mi.GetParameters();
//                            if (parameterInfos == null || parameterInfos.Length == 0)
//                            {
//                                sb.Append("Empty,");
//                            }
//                            else
//                            {
//                                foreach (var pi in parameterInfos)
//                                {
//                                    sb.Append(pi.ParameterType.FullName + ",");
//                                }
//                            }
//                            string paramJoinName = sb.ToString();
//                            if (paramJoinName.Length > 0)
//                            {
//                                paramJoinName = paramJoinName.Substring(0, paramJoinName.Length - 1);
//                            }
//                            if (!mps.Contains(paramJoinName))
//                            {
//                                mps.Add(paramJoinName);
//                            }
//                        }
//                    }

//                    methodParamJoinNames = mps.ToArray();
//                }
//            }
//            if (methodParamJoinNames.Length > 0)
//            {
//                if (Array.IndexOf(methodParamJoinNames, methodParamJoinName) < 0)
//                {
//                    methodParamJoinName = methodParamJoinNames[0];
//                }
//            }
//            else
//            {
//                methodParamJoinName = "";
//            }
//        }

//        private string[] GetMethodParamJoinName()
//        {
//            return methodParamJoinNames;
//        }

//        public override bool Equals(object obj)
//        {
//            if (!(obj is GenBlackData data))
//                return false;
//            return data.typeName == typeName && data.memberType == memberType && data.memberInfoName == memberInfoName && data.methodParamJoinName == methodParamJoinName;
//        }

//        public override int GetHashCode()
//        {
//            var hashCode = -572694141;
//            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(typeName);
//            hashCode = hashCode * -1521134295 + memberType.GetHashCode();
//            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(memberInfoName);
//            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(methodParamJoinName);
//            return hashCode;
//        }
//    }
//}
