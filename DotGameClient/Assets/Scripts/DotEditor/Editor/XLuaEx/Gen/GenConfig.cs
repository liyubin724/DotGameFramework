//using Dot.XLuaEx.Gen;
//using DotEditor.Core.Util;
//using DotGame.XLuaEx;
//using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
//using System.IO;
//using System.Reflection;
//using System.Text;
//using UnityEditor;
//using UnityEditor.Callbacks;
using UnityEngine;
using XLua;

namespace DotEditor.XLuaEx
{
    public static class GenConfig
    {
        //lua中要使用到C#库的配置，比如C#标准库，或者Unity API，第三方库等。
        [LuaCallCSharp]
        public static List<Type> LuaCallCSharp = new List<Type>() {
                typeof(System.Object),
                typeof(UnityEngine.Object),
                typeof(Vector2),
                typeof(Vector3),
                typeof(Vector4),
                typeof(Quaternion),
                typeof(Color),
                typeof(Ray),
                typeof(Bounds),
                typeof(Ray2D),
                typeof(Time),
                typeof(GameObject),
                typeof(Component),
                typeof(Behaviour),
                typeof(Transform),
                typeof(Resources),
                typeof(TextAsset),
                typeof(Keyframe),
                typeof(AnimationCurve),
                typeof(AnimationClip),
                typeof(MonoBehaviour),
                typeof(ParticleSystem),
                typeof(SkinnedMeshRenderer),
                typeof(Renderer),
                typeof(Light),
                typeof(Mathf),
                typeof(System.Collections.Generic.List<int>),
                typeof(Action<string>),
                typeof(UnityEngine.Debug)
            };

        //C#静态调用Lua的配置（包括事件的原型），仅可以配delegate，interface
        [CSharpCallLua]
        public static List<Type> CSharpCallLua = new List<Type>() {
                typeof(Action),
                typeof(Func<double, double, double>),
                typeof(Action<string>),
                typeof(Action<double>),
                typeof(Action<float>),
                typeof(UnityEngine.Events.UnityAction),
                typeof(System.Collections.IEnumerator)
            };

        //黑名单
        [BlackList]
        public static List<List<string>> BlackList = new List<List<string>>()  {
                new List<string>(){"System.Xml.XmlNodeList", "ItemOf"},
                new List<string>(){"UnityEngine.WWW", "movie"},
    #if UNITY_WEBGL
                new List<string>(){"UnityEngine.WWW", "threadPriority"},
    #endif
                new List<string>(){"UnityEngine.Texture2D", "alphaIsTransparency"},
                new List<string>(){"UnityEngine.Security", "GetChainOfTrustValue"},
                new List<string>(){"UnityEngine.CanvasRenderer", "onRequestRebuild"},
                new List<string>(){"UnityEngine.Light", "areaSize"},
                new List<string>(){"UnityEngine.Light", "lightmapBakeType"},
                new List<string>(){"UnityEngine.WWW", "MovieTexture"},
                new List<string>(){"UnityEngine.WWW", "GetMovieTexture"},
                new List<string>(){"UnityEngine.AnimatorOverrideController", "PerformOverrideClipListCleanup"},
    #if !UNITY_WEBPLAYER
                new List<string>(){"UnityEngine.Application", "ExternalEval"},
    #endif
                new List<string>(){"UnityEngine.GameObject", "networkView"}, //4.6.2 not support
                new List<string>(){"UnityEngine.Component", "networkView"},  //4.6.2 not support
                new List<string>(){"System.IO.FileInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
                new List<string>(){"System.IO.FileInfo", "SetAccessControl", "System.Security.AccessControl.FileSecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
                new List<string>(){"System.IO.DirectoryInfo", "SetAccessControl", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "CreateSubdirectory", "System.String", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "Create", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"UnityEngine.MonoBehaviour", "runInEditMode"},
            };
    }












    //        private static readonly string GenConfigPath = "Assets/Tools/XLua/gen_config.asset";

    //        [MenuItem("Game/XLuaEx/Create Gen Config", false, 2)]
    //        public static void CreateGenConfig()
    //        {
    //            GenConfigData data = AssetDatabase.LoadAssetAtPath<GenConfigData>(GenConfigPath);
    //            if (data == null)
    //            {
    //                data = ScriptableObject.CreateInstance<GenConfigData>();
    //                AssetDatabase.CreateAsset(data, GenConfigPath);
    //            }
    //            Selection.activeObject = data;
    //        }

    //        [MenuItem("Game/XLuaEx/Export Gen Types", false, 3)]
    //        public static void BrowseExportTypes()
    //        {
    //            StringBuilder sb = new StringBuilder();
    //            var assembies = AppDomain.CurrentDomain.GetAssemblies();
    //            foreach (var assembly in assembies)
    //            {
    //                sb.AppendLine("------------------------");
    //                sb.AppendLine(assembly.GetName().FullName);
    //                sb.AppendLine("------------------------");
    //                Type[] types = assembly.GetTypes();
    //                foreach (var t in types)
    //                {
    //                    if(t.IsPublic)
    //                        sb.AppendLine(t.FullName);
    //                }
    //                sb.AppendLine();
    //                sb.AppendLine();
    //            }

    //            string filePath = "D:/xlua_export_type.txt";
    //            File.WriteAllText(filePath, sb.ToString());

    //            ExplorerUtil.OpenExplorerFile(filePath);
    //        }

    //        [MenuItem("Game/XLuaEx/Explort All Assemblies", false, 4)]
    //        public static void PrintAssembies()
    //        {
    //            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
    //            StringBuilder sb = new StringBuilder();
    //            foreach (var assembly in assemblies)
    //            {
    //                if (assembly.IsDynamic || string.IsNullOrEmpty(assembly.Location))
    //                {
    //                    continue;
    //                }
    //                sb.AppendLine(assembly.Location);
    //            }

    //            string filePath = "D:/xlua_export_assemblies.txt";
    //            File.WriteAllText(filePath, sb.ToString());

    //            ExplorerUtil.OpenExplorerFile(filePath);
    //        }

    //        [DidReloadScripts]
    //        public static void FilterDll()
    //        {
    //            GenConfigData data = ConfigData;
    //            List<string> allClassTypeList = new List<string>();
    //            List<string> allValueTypeList = new List<string>();
    //            foreach (var dllName in data.genDllNameList)
    //            {
    //                if(string.IsNullOrEmpty(dllName))
    //                {
    //                    continue;
    //                }
    //                var types = Assembly.Load(dllName).GetTypes();
    //                foreach (var t in types)
    //                {
    //                    Attribute attr = t.GetCustomAttribute<ObsoleteAttribute>();
    //                    if (attr != null)
    //                    {
    //                        continue;
    //                    }
    //                    attr = t.GetCustomAttribute<NoGenAttribute>();
    //                    if (attr != null)
    //                    {
    //                        continue;
    //                    }

    //                    if(data.excludeNamespaceList.IndexOf(t.Namespace)>=0)
    //                    {
    //                        continue;
    //                    }
    //                    if(!t.IsPublic)
    //                    {
    //                        continue;
    //                    }
    //                    if(t.IsInterface)
    //                    {
    //                        continue;
    //                    }

    //                    allClassTypeList.Add(t.FullName);

    //                    if (t.IsValueType)
    //                        allValueTypeList.Add(t.FullName);
    //                }
    //            }
    //            allClassTypeList.Sort();
    //            allValueTypeList.Sort();
    //            allClassTypeNameArr = allClassTypeList.ToArray();
    //            allValueTypeNameArr = allValueTypeList.ToArray();
    //        }

    //        private static string[] allClassTypeNameArr = null;
    //        private static string[] allValueTypeNameArr = null;
    //        internal static string[] AllClassTypeNames
    //        {
    //            get
    //            {
    //                return allClassTypeNameArr;
    //            }
    //        }

    //        internal static string[] AllValueTypeNames
    //        {
    //            get
    //            {
    //                return allValueTypeNameArr;
    //            }
    //        }

    //        private static GenConfigData ConfigData
    //        {
    //            get
    //            {
    //                GenConfigData cData = AssetDatabase.LoadAssetAtPath<GenConfigData>(GenConfigPath);
    //                if(cData == null)
    //                {
    //                    CreateGenConfig();
    //                    return AssetDatabase.LoadAssetAtPath<GenConfigData>(GenConfigPath);
    //                }

    //                return cData;
    //            }
    //        }

    //        [GCOptimize]
    //        public static List<Type> GCOptimize
    //        {
    //            get
    //            {
    //                return GetTypes(ConfigData.gcOptimizeTypeNameList);
    //            }
    //        }

    //        [LuaCallCSharp]
    //        public static List<Type> LuaCallCSharpTypes
    //        {
    //            get
    //            {
    //                List<Type> luaCallList = new List<Type>();
    //                luaCallList.AddRange(GetTypes(ConfigData.luaCallCSharpTypeNameList));
    //                return luaCallList;
    //            }
    //        }

    //        [CSharpCallLua]
    //        public static List<Type> CSharpCallLuaTypes
    //        {
    //            get
    //            {
    //                List<Type> csharpCallList = new List<Type>();
    //                csharpCallList.AddRange(GetTypes(ConfigData.csharpCallLuaTypeNameList));
    //                csharpCallList.Add(typeof(Action<float>));
    //                return csharpCallList;
    //            }
    //        }

    //        [BlackList]
    //        public static List<List<string>> BlackInfoList
    //        {
    //            get
    //            {
    //                List<GenBlackData> blackDataList = ConfigData.blackTypeInfoList;
    //                List<List<string>> blackList = new List<List<string>>();
    //                foreach (var data in blackDataList)
    //                {
    //                    List<string> singleBlack = new List<string>();
    //                    if (!string.IsNullOrEmpty(data.typeName) && !string.IsNullOrEmpty(data.memberInfoName))
    //                    {
    //                        singleBlack.Add(data.typeName);
    //                        singleBlack.Add(data.memberInfoName);
    //                        if (data.memberType == GenBlackMemberType.Method)
    //                        {
    //                            if (!string.IsNullOrEmpty(data.methodParamJoinName) && data.methodParamJoinName != "Empty")
    //                            {
    //                                string[] paramsNames = data.methodParamJoinName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
    //                                foreach (var pn in paramsNames)
    //                                {
    //                                    singleBlack.Add(pn);
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //                return blackList;
    //            }
    //        }

    //        internal static List<Type> GetTypes(List<string> types)
    //        {
    //            var assembies = AppDomain.CurrentDomain.GetAssemblies();
    //            List<Type> result = new List<Type>();
    //            foreach (var t in types)
    //            {
    //                foreach (var a in assembies)
    //                {
    //                    var type = a.GetType(t);
    //                    if (type != null)
    //                    {
    //                        result.Add(type);
    //                        break;
    //                    }
    //                }
    //            }
    //            return result;
    //        }
}
