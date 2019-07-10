using System.Collections.Generic;
using UnityEngine.Events;

namespace Dot.Core.Pool
{
    public class ObjectPool<T> where T : class,new()
    {
        private Stack<T> m_Stack = new Stack<T>();
        private UnityAction<T> m_ActionOnNew;   
        private UnityAction<T> m_ActionOnRealse;

        public int Count { get; private set; }
        public int ActiveCount { get { return Count - InactiveCount; } }
        public int InactiveCount { get { return m_Stack.Count; } }

        public ObjectPool(UnityAction<T> onNewAction,UnityAction<T> onReleaseAction,int preloadCount=0)
        {
            m_ActionOnNew = onNewAction;
            m_ActionOnRealse = onReleaseAction;
            if(preloadCount>0)
            {
                for (int i = 0; i < preloadCount; i++)
                {
                    T element = new T();
                    m_Stack.Push(element);

                    ++Count;

                    m_ActionOnNew?.Invoke(element);
                }
            }
        }

        public T Get()
        {
            T element = default;
            if (m_Stack.Count == 0)
            {
                element = new T();
                ++Count;
                m_ActionOnNew?.Invoke(element);
            }
            else
            {
                element = m_Stack.Pop();
            }
            return element;
        }

        public void Release(T element)
        {
            if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
                UnityEngine.Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");

            m_ActionOnRealse?.Invoke(element);
            m_Stack.Push(element);
        }

        public void Clear()
        {
            m_Stack.Clear();
            m_Stack = null;
            m_ActionOnNew = null;
            m_ActionOnRealse = null;
        }
    }
}
