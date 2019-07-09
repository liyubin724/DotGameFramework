using Dot.Core.Event;
using Dot.Core.Timer;
using Dot.XLuaEx;
using XLua;

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

        public static GameLua GLua
        {
            get
            {
                return InnerGameController.GetInstance().GLua;
            }
        }

        public static LuaEnv GLuaEnv
        {
            get
            {
                return GLua.GameLuaEnv;
            }
        }
    }
}
