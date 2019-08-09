using Dot.Config;
using Dot.Core.Effect;
using Dot.Core.TimeLine;
using UnityEngine;

namespace Dot.Core.Entity.TimeLine.Game
{
    [TimeLineMark("Event/Effect", "At Position", TimeLineExportPlatform.Client)]
    public class EffectAtPositionEvent : AEventItem
    {
        public int ConfigID { get; set; }
        public bool UseEntityPosition { get; set; } = false;
        public Vector3 Position { get; set; }

        public override void DoRevert()
        {
           
        }

        public override void Trigger()
        {
            EffectConfigData data = ConfigManager.GetInstance().GetEffectConfig(ConfigID);
            EffectController effect = EffectManager.GetInstance().GetEffect(data.address,EffectScenarioType.Timline,true);
            effect.isAutoPlayWhenEnable = data.isAutoPlay;
            effect.lifeTime = data.lifeTime;
            effect.stopDelayTime = data.stopDelayTime;

            if(UseEntityPosition)
            {
                effect.CachedTransform.position = entity.EntityData.GetPosition();
            }
            else
            {
                effect.CachedTransform.position = Position;
            }
        }
    }
}
