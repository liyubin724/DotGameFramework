using Dot.Core.Logger;
using System.Collections;
using System.Collections.Generic;

namespace Dot.Core.Pool
{
    public class ObjectPool
    {
        private Stack m_Stack = new Stack();

        public int Count { get; private set; }
        public int ActiveCount { get { return Count - InactiveCount; } }
        public int InactiveCount { get { return m_Stack.Count; } }

        public ObjectPool()
        {
        }

        public T Get<T>() where T:IObjectPoolItem,new()
        {
            T element = default;
            if (m_Stack.Count == 0)
            {
                element = new T();
                ++Count;
                element.OnNew();
            }
            else
            {
                element = (T)m_Stack.Pop();
            }
            return element;
        }

        public void Release<T>(T element) where T:IObjectPoolItem
        {
            if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
                DebugLogger.LogError("Internal error. Trying to destroy object that is already released to pool.");

            element.OnRelease();

            m_Stack.Push(element);
        }

        public void Clear()
        {
            m_Stack.Clear();
            m_Stack = null;
        }
    }

    public class ObjectPool<T> where T : class,IObjectPoolItem,new()
    {
        private Stack<T> m_Stack = new Stack<T>();

        public int Count { get; private set; }
        public int ActiveCount { get { return Count - InactiveCount; } }
        public int InactiveCount { get { return m_Stack.Count; } }

        public ObjectPool(int preloadCount=0)
        {
            if(preloadCount>0)
            {
                for (int i = 0; i < preloadCount; i++)
                {
                    T element = new T();
                    element.OnNew();
                    m_Stack.Push(element);

                    ++Count;
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
                element.OnNew();
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
                DebugLogger.LogError("Internal error. Trying to destroy object that is already released to pool.");

            element.OnRelease();

            m_Stack.Push(element);
        }

        public void Clear()
        {
            m_Stack.Clear();
            m_Stack = null;
        }
    }
}
