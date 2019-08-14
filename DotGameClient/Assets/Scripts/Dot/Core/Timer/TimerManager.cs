using Dot.Core.Util;

namespace Dot.Core.Timer
{
    public delegate void TimerCallback(object obj);

    public class TimerManager : Singleton<TimerManager>
    {
        private HierarchicalTimerWheel hTimerWheel = null;
        private bool isPause = false;

        protected override void DoInit()
        {
            hTimerWheel = new HierarchicalTimerWheel();
        }

        public override void DoReset()
        {
            hTimerWheel?.Clear();
            isPause = false;
        }
        
        public void DoUpdate(float deltaTime)
        {
            if (!isPause && hTimerWheel != null)
            {
                hTimerWheel.OnUpdate(deltaTime);
            }
        }

        public void Pause()
        {
            isPause = true;
        }

        public void Resume()
        {
            isPause = false;
        }

        public TimerTaskInfo AddTimer(float intervalInSec,
                                                float totalInSec,
                                                TimerCallback startCallback,
                                                TimerCallback intervalCallback,
                                                TimerCallback endCallback,
                                                object callbackData)
        {
            if (hTimerWheel == null) return null;

            TimerTask task = hTimerWheel.GetIdleTimerTask();
            task.OnReused(intervalInSec, totalInSec, startCallback, intervalCallback, endCallback, callbackData);
            return hTimerWheel.AddTimerTask(task);
        }

        public TimerTaskInfo AddTimer(float intervalInSec,
                                                float totalInSec,
                                                TimerCallback intervalCallback,
                                                TimerCallback endCallback,
                                                object callbackData = null)
        {
            return AddTimer(intervalInSec, totalInSec, null, intervalCallback, endCallback, callbackData);
        }

        public TimerTaskInfo AddIntervalTimer(float intervalInSec, TimerCallback intervalCallback, object callbackData = null)
        {
            return AddTimer(intervalInSec, 0f, null, intervalCallback, null, callbackData);
        }

        public TimerTaskInfo AddEndTimer(float totalInSec, TimerCallback endCallback,object callbackData = null)
        {
            return AddTimer(totalInSec, totalInSec, null, null, endCallback, callbackData);
        }

        public bool RemoveTimer(TimerTaskInfo taskInfo)
        {
            if (hTimerWheel != null)
            {
                return hTimerWheel.RemoveTimerTask(taskInfo);
            }
            return false;
        }

        public override void DoDispose()
        {
            DoReset();
            hTimerWheel = null;
        }
    }
}
