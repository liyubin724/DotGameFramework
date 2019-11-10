using Dot.Core.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Core.World
{
    public partial class WorldManager
    {
        private EventDispatcher dispatcher = null;

        private void DoInitEvent()
        {
            dispatcher = new EventDispatcher();
        }
    }
}
