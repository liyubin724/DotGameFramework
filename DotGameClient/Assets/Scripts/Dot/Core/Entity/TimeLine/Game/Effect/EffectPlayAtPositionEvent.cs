using Dot.Config;
using Dot.Core.Effect;
using Dot.Core.TimeLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dot.Core.Entity.TimeLine.Game
{
    public class EffectPlayAtPositionEvent : AEventItem
    {
        public int ConfigID { get; set; }
        
        public Vector3 Position { get; set; }

        public override void DoRevert()
        {
           
        }

        public override void Trigger()
        {
            EffectConfigData data = ConfigManager.GetInstance().GetEffectConfig(ConfigID);
            EffectController effect = EffectManager.GetInstance().GetEffect(data.address);
            effect.isAutoPlayWhenEnable = data.isAutoPlay;
            effect.lifeTime = data.lifeTime;
            effect.stopDelayTime = data.stopDelayTime;

            effect.CachedTransform.position = Position;
        }
    }
}
