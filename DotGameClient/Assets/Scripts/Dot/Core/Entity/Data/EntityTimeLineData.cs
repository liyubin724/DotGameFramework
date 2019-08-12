using Dot.Core.TimeLine;
using Dot.Core.TimeLine.Data;
using LitJson;

namespace Dot.Core.Entity.Data
{
    public class EntityTimeLineData
    {
        internal EntityBaseData baseData = null;

        public EntityTimeLineData(EntityBaseData baseData)
        {
            this.baseData = baseData;
        }

        private TrackController trackController = null;
        public TrackController GetTrackControl()=> trackController;

        public void InitTrack(string jsonText)
        {
            if (string.IsNullOrEmpty(jsonText))
            {
                return;
            }

            trackController = JsonDataReader.ReadData(JsonMapper.ToObject(jsonText));
            baseData.SendEvent(EntityInnerEventConst.TIMELINE_ADD_ID);
        }

        public void FinishTrack()
        {
            trackController = null;
        }

    }
}
