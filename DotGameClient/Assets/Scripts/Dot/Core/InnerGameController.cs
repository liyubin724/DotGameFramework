using Dot.Core.Event;
using Dot.Core.Timer;
using Dot.XLuaEx;
using UnityEngine;

namespace Dot.Core
{
    
    public class InnerGameController : MonoBehaviour
    {
        private void Awake()
        {
            if(igc!=null)
            {
                Destroy(this);
            }else
            {
                igc = this;
            }
        }

        private void Update()
        {
            if(timer!=null)
            {
                timer.DoUpdate(Time.deltaTime);
            }
        }

        internal void DoDispose()
        {
            eventDispatcher.DoDispose();
            eventDispatcher = null;
            timer.DoDispose();
            timer = null;

            igc = null;
            Destroy(gameObject);
        }

        internal void DoReset()
        {
            eventDispatcher.DoReset();
            timer.DoReset();
        }

        private GameTimer timer = null;
        internal GameTimer GTimer
        {
            get
            {
                if(timer == null)
                {
                    timer = new GameTimer();
                }
                return timer;
            }
        }

        private EventDispatcher eventDispatcher = null;
        internal EventDispatcher GEvent
        {
            get
            {
                if(eventDispatcher == null)
                {
                    eventDispatcher = new EventDispatcher();
                }
                return eventDispatcher;
            }
        }

        private GameLua gameLua = null;
        internal GameLua GLua
        {
            get
            {
                if(gameLua == null)
                {
                    gameLua = new GameLua();
                }
                return gameLua;
            }
        }

        private static InnerGameController igc = null;
        internal static InnerGameController GetInstance()
        {
            if(igc==null)
            {
                GameObject igcGO = new GameObject("__igc");
                //igcGO.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
                igc = igcGO.AddComponent<InnerGameController>();
                GameObject.DontDestroyOnLoad(igcGO);
            }
            return igc;
        }
    }
}
