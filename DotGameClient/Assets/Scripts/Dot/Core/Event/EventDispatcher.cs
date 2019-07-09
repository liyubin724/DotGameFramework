using Dot.Core.Pool;
using Dot.Core.Timer;
using System.Collections.Generic;
using SystemObject = System.Object;

namespace Dot.Core.Event
{
    public delegate void EventHandler(EventData e);

    public class EventDispatcher
    {
        private static ObjectPool<EventData> eventDataPool = null;
        private Dictionary<int, List<EventHandler>> eventHandlerDic = null;
        private Dictionary<EventData, TimerTaskInfo> delayEventTaskInfo = null;
        public EventDispatcher()
        {
            if(eventDataPool == null)
            {
                eventDataPool = new ObjectPool<EventData>(null, (eventData) =>
                {
                    eventData.SetData(-1, 0f, null);
                }, 10);
            }
            
            eventHandlerDic = new Dictionary<int, List<EventHandler>>();
            delayEventTaskInfo = new Dictionary<EventData, TimerTaskInfo>();
        }

        public void DoReset()
        {
            foreach(var kvp in eventHandlerDic)
            {
                kvp.Value.Clear();
            }
            foreach(var kvp in delayEventTaskInfo)
            {
                eventDataPool.Release(kvp.Key);
                GameApplication.GTimer.RemoveTimerTask(kvp.Value);
            }
            delayEventTaskInfo.Clear();
        }

        public void DoDispose()
        {
            DoReset();
            eventDataPool?.Clear();
            eventDataPool = null;
            eventHandlerDic = null;
            delayEventTaskInfo = null;
        }

        public void RegisterEvent(int eventID, EventHandler handler)
        {
            if (!eventHandlerDic.TryGetValue(eventID, out List<EventHandler> handlerList))
            {
                handlerList = new List<EventHandler>();
                eventHandlerDic.Add(eventID, handlerList);
            }

            handlerList.Add(handler);
        }

        public void UnregisterEvent(int eventID, EventHandler handler)
        {
            if (eventHandlerDic.TryGetValue(eventID, out List<EventHandler> handlerList))
            {
                if (handlerList != null)
                {
                    for (int i = handlerList.Count - 1; i >= 0; i--)
                    {
                        if (handlerList[i] == null || handlerList[i] == handler)
                        {
                            handlerList.RemoveAt(i);
                        }
                    }
                }
            }
        }

        public void TriggerEvent(int eventID)
        {
            TriggerEvent(eventID, 0.0f, null);
        }

        public void TriggerEvent(int eventID, float delayTime)
        {
            TriggerEvent(eventID, delayTime, null);
        }

        public void TriggerEvent(int eventID, float delayTime, params object[] datas)
        {
            EventData e = eventDataPool.Get();
            e.SetData(eventID, delayTime, datas);

            if (e.EventDelayTime <= 0)
            {
                TriggerEvent(e);
            }
            else
            {
                TimerTaskInfo tti = GameApplication.GTimer.AddTimerTask(
                    delayTime, 
                    delayTime, null, null, 
                    OnDelayEventTrigger, e);
                delayEventTaskInfo.Add(e,tti);
            }
        }

        private void OnDelayEventTrigger(object userdata)
        {
            if(userdata!=null && userdata is EventData eData)
            {
                delayEventTaskInfo.Remove(eData);
                TriggerEvent(eData);
            }
        }

        private void TriggerEvent(EventData e)
        {
            if (eventHandlerDic.TryGetValue(e.EventID, out List<EventHandler> handlerList))
            {
                if (handlerList != null && handlerList.Count > 0)
                {
                    for (var i = handlerList.Count - 1; i >= 0; --i)
                    {
                        if (handlerList[i] == null)
                        {
                            handlerList.RemoveAt(i);
                        }
                        else
                        {
                            handlerList[i](e);
                        }
                    }
                    eventDataPool.Release(e);
                }
            }
        }
    }

    public class EventData
    {
        private int eventID = -1;
        private float eventDelayTime = 0.0f;
        private SystemObject[] eventParams = null;

        public int EventID => eventID;
        public float EventDelayTime => eventDelayTime;
        public int ParamCount => eventParams == null ? 0 : eventParams.Length;
        public SystemObject[] EventParams => eventParams;
        
        public EventData() { }

        internal void SetData(int eID, float eDelayTime, params object[] objs)
        {
            eventID = eID;
            eventDelayTime = eDelayTime;
            eventParams = objs;
        }

        public T GetValue<T>(int index = 0)
        {
            object result = GetObjectValue(index);
            if (result == null)
                return default;
            else
                return (T)result;
        }

        public object GetObjectValue(int index = 0)
        {
            if (eventParams == null || eventParams.Length == 0)
            {
                return null;
            }
            if (index < 0 || index >= eventParams.Length)
            {
                return null;
            }

            return eventParams[index];
        }
    }
}
