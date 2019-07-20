using Dot.Core.TimeLine;
using UnityEngine;

namespace Game.TimeLine
{
    [TimeLineMark("Event/Effect", "Add Effect At Position", TimeLineExportPlatform.Client)]
    public class AddEffectAtPositionAction : AActionItem
    {
        public int ConfigID { get; set; }
        public Vector3 Position { get; set; }

        private GameEntity effectEntity = null;
        public override void Enter()
        {
            effectEntity = services.entityFactroy.CreateEffectEntity(GetGameEntity(), ConfigID);
            effectEntity.AddPosition(Position);
        }

        public override void Exit()
        {
            effectEntity.isMarkDestroy = true;
        }
    }
}
