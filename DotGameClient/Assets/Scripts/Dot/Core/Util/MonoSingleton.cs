﻿using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Util
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T:MonoSingleton<T>
    {
        protected static T instance = null;
        public static T GetInstance()
        {
            if (instance == null)
            {
                instance = UnityObject.FindObjectOfType<T>();
                if(instance == null)
                {
                    instance = DontDestoryHandler.CreateComponent<T>();
                }else
                {
                    DontDestoryHandler.AddTransform(instance.transform);
                }
            }
            return instance;
        }
    }
}
