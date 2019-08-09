using Dot.Core.Effect;
using Dot.Core.Entity.Controller;
using Dot.Core.TimeLine;

namespace Dot.Core.Entity.TimeLine.Game
{
    [TimeLineMark("Event/Effect", "Bind Node", TimeLineExportPlatform.Client)]
    public class EffectBindNodeEvent : AEventItem
    {
        public int ConfigID { get; set; }
        public BindNodeType NodeType { get; set; } = BindNodeType.Main;
        public bool IsAllNode { get; set; } = false;
        public int NodeIndex { get; set; }

        public override void DoRevert()
        {
            
        }

        public override void Trigger()
        {
            EntityEffectController effectController = entity.GetController<EntityEffectController>(EntityControllerConst.EFFECT_INDEX);
            if (effectController == null) return;

            if(IsAllNode)
            {
                effectController.BindEffect(NodeType, ConfigID, EffectScenarioType.Timline,true);
            }else
            {
                effectController.BindEffect(NodeType,NodeIndex, ConfigID, EffectScenarioType.Timline, true);
            }
        }
    }
}
