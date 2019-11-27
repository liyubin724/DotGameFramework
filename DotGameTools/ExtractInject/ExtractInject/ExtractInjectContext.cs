using System;
using System.Collections.Generic;

namespace ExtractInject
{
    public class ExtractInjectContext : IExtractInjectContext
    {
        private Dictionary<Type, IExtractInjectObject> m_ObjectDic;

        public ExtractInjectContext()
        {
            m_ObjectDic = new Dictionary<Type, IExtractInjectObject>();
        }

        public ExtractInjectContext(params IExtractInjectObject[] objs) : this()
        {
            if(objs != null)
            {
                foreach (var obj in objs)
                {
                    if (obj != null)
                    {
                        AddObject(obj);
                    }
                }
            }
        }

        public bool ContainsObject<T>() where T : IExtractInjectObject
        {
            return ContainsObject(typeof(T));
        }

        public bool ContainsObject(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("ExtractInjectContext:ContainsObject->Argument is Null");

            return m_ObjectDic.ContainsKey(type);
        }

        public T GetObject<T>() where T : IExtractInjectObject
        {
            return (T)m_ObjectDic[typeof(T)];
        }

        public IExtractInjectObject GetObject(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("ExtractInjectContext:GetObject->Argument is Null");

            return m_ObjectDic[type];
        }

        public void AddObject(IExtractInjectObject obj)
        {
            if (obj == null)
                return;
            AddObject(obj.GetType(), obj);
        }

        public void AddObject<T>(T obj) where T : IExtractInjectObject
        {
            if (obj == null)
                return;

            AddObject(typeof(T), obj);
        }

        public void AddObject(Type type, IExtractInjectObject obj)
        {
            if(obj== null)
            {
                throw new ArgumentNullException("ExtractInjectContext::AddObject->obj is null");
            }

            if (!type.IsInstanceOfType(obj))
            {
                throw new InvalidOperationException($"'{obj.GetType()}' is not of passed in type '{type}'.");
            }

            if (m_ObjectDic.ContainsKey(type))
            {
                m_ObjectDic[type] = obj;
            }
            else
            {
                m_ObjectDic.Add(type, obj);
            }
        }

        public bool TryGetObject<T>(out T obj) where T : IExtractInjectObject
        {
            if (m_ObjectDic.TryGetValue(typeof(T), out IExtractInjectObject value))
            {
                obj = (T)value;
                return true;
            }

            obj = default;
            return false;
        }

        public bool TryGetObject(Type type, out IExtractInjectObject obj)
        {
            if(m_ObjectDic.TryGetValue(type,out IExtractInjectObject value) && type.IsInstanceOfType(value))
            {
                obj = value;
                return true;
            }
            obj = null;
            return false;
        }

        public void DeleteObject<T>() where T : IExtractInjectObject
        {
            DeleteObject(typeof(T));
        }

        public void DeleteObject(IExtractInjectObject obj)
        {
            DeleteObject(obj.GetType());
        }

        public void DeleteObject(Type type)
        {
            if(m_ObjectDic.ContainsKey(type))
            {
                m_ObjectDic.Remove(type);
            }
        }
    }
}
