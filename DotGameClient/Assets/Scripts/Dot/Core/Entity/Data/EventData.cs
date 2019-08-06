using Dot.Core.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemObject = System.Object;

namespace Dot.Core.Entity.Data
{
    public class EventData
    {
        private EventDispatcher dispatcher;
        internal EventDispatcher Dispatcher { set => dispatcher = value; }
        internal void SendEvent(int eventID, SystemObject[] datas = null) => dispatcher?.TriggerEvent(eventID, 0, datas);
    }
}
