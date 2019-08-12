using Dot.Core.Event;
using Dot.Core.Timer;

namespace Dot.Core
{
    public static  class GameApplication
    {
        public static GameTimer GTimer
        {
            get
            {
                return InnerGameController.GetInstance().GTimer;
            }
        }

        public static EventDispatcher GEvent
        {
            get
            {
                return InnerGameController.GetInstance().GEvent;
            }
        }
    }
}
