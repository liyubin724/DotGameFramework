using Dot.Core.Event;
using Dot.Core.TimeLine;
using Dot.Core.TimeLine.Data;
using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Core.Entity.Data
{
    public class EntityTimeLineData
    {
        internal EntityEventData eventData = null;
        
        private TrackController trackController = null;
        public void SetTrackControl(string jsonText)
        {
            if(string.IsNullOrEmpty(jsonText))
            {
                return;
            }

            trackController = JsonDataReader.ReadData(JsonMapper.ToObject(jsonText));
            eventData.SendEvent(EntityInnerEventConst.TIMELINE_ADD_ID);
        }

        public TrackController GetTrackControl()=> trackController;

        public void TrackFinish()
        {

        }
    }
}
