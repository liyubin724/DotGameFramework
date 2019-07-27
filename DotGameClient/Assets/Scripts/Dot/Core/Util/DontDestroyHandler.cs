using UnityEngine;

namespace Dot.Core.Util
{
    public static class DontDestroyHandler
    {
        private static readonly string RootName = "Singleton Root";
        private static Transform rootTran = null;

        private static Transform RootTransform
        {
            get
            {
                if(rootTran == null)
                {
                    CreateRootTransform();
                }

                return rootTran;
            }
        }

        private static void CreateRootTransform()
        {
            GameObject rootGO = GameObject.Find(RootName);
            if (rootGO == null)
            {
                rootGO = new GameObject(RootName);
            }
            Object.DontDestroyOnLoad(rootGO);
            rootTran = rootGO.transform;
        }

        public static Transform CreateTransform(string name)
        {
            GameObject behGO = new GameObject(name);
            Transform tran = behGO.transform;
            AddTransform(tran);
            return tran;
        }

        public static void AddTransform(Transform tran)
        {
            tran.parent = RootTransform;
            tran.localPosition = Vector3.zero;
            tran.localScale = Vector3.one;
            tran.localEulerAngles = Vector3.zero;
        }

        public static T CreateComponent<T>() where T : MonoBehaviour
        {
            T component = RootTransform.GetComponentInChildren<T>();
            if(component == null)
            {
                Transform tran = CreateTransform(typeof(T).Name);
                component = tran.gameObject.AddComponent<T>();
            }

            return component;
        }

        public static void Destroy()
        {
            if(rootTran!=null)
            {
                Object.Destroy(rootTran.gameObject);
            }
            rootTran = null;
        }
    }
}
