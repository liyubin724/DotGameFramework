using Dot.Core.Manager;

namespace Dot.Core.Timer
{
    public delegate void TimerCallback(object obj);

    public class GameTimer
    {
        private HierarchicalTimerWheel hTimerWheel = null;
        private bool isPause = false;

        public GameTimer()
        {
            hTimerWheel = new HierarchicalTimerWheel();
        }

        public void DoReset()
        {
            if (hTimerWheel != null)
            {
                hTimerWheel.Clear();
            }
            isPause = false;
        }

        public void DoUpdate(float deltaTime)
        {
            if (!isPause && hTimerWheel != null)
            {
                hTimerWheel.OnUpdate(deltaTime);
            }
        }

        public void PauseTimer()
        {
            isPause = true;
        }

        public void ResumeTimer()
        {
            isPause = false;
        }

        public TimerTaskInfo AddTimerTask(float intervalInSec,
                                                float totalInSec,
                                                TimerCallback startCallback,
                                                TimerCallback intervalCallback,
                                                TimerCallback endCallback,
                                                object callbackData)
        {
            TimerTask task = hTimerWheel.GetIdleTimerTask();
            task.OnReused(intervalInSec, totalInSec, startCallback, intervalCallback, endCallback, callbackData);
            return hTimerWheel.AddTimerTask(task);
        }

        public bool RemoveTimerTask(TimerTaskInfo taskInfo)
        {
            return hTimerWheel.RemoveTimerTask(taskInfo);
        }

        public void DoDispose()
        {
            DoReset();
            hTimerWheel = null;
        }
    }
}
