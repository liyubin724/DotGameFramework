using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entitas;

namespace Game.TimeLine.Systems
{
    public class TimeLineSystem : AGameEntitySystem,IInitializeSystem
    {
        private IGroup<GameEntity> timelineDataAddGroup = null;
        private IGroup<GameEntity> timelineDataRemoveGroup = null;
        public TimeLineSystem(Contexts contexts, Services services) : base(contexts, services)
        {
        }

        public void Initialize()
        {
            
        }
    }
}
