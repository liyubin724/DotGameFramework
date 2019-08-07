using Dot.Core.Event;
using Dot.Core.TimeLine;

namespace Dot.Core.Entity.Controller
{
    public abstract class EntityTimeLineController : AEntityController
    {
        protected override void AddEventListeners()
        {
            entity.RegisterEvent(EntityInnerEventConst.TIMELINE_ADD_ID,OnAddTimeLine);
        }

        protected override void RemoveEventListeners()
        {
            entity.UnregisterEvent(EntityInnerEventConst.TIMELINE_ADD_ID, OnAddTimeLine);
        }

        public override void DoUpdate(float deltaTime)
        {
            if(trackController!=null && trackController.IsPlaying())
            {
                trackController.DoUpdate(deltaTime);
            }
        }

        protected TrackController trackController = null;
        private void OnAddTimeLine(EventData userData)
        {
            if(trackController!=null && trackController.IsPlaying())
            {
                trackController.Stop(true);
                trackController.DoReset();
            }

            trackController = entity.EntityData.TimeLineData.GetTrackControl();
            trackController.Initialize(context, entity);
            trackController.groupStartCallback += OnTrackGroupStart;
            trackController.groupChangedCallback += OnTrackGroupChanged;
            trackController.groupFinishCallback += OnTrackGroupFinish;
        }

        public void Play(string groupName)
        {
            if(trackController!=null && !trackController.IsPlaying(groupName))
            {
                trackController.Play(groupName);
            }
        }

        public void Stop()
        {
            if (trackController != null && trackController.IsPlaying())
            {
                trackController.Stop(true);
            }
        }
        
        protected virtual  void OnTrackGroupStart(EntityObject entity,string groupName)
        {
            entity.SendEvent(EntityInnerEventConst.TIMELINE_GROUP_START_ID, groupName);
        }

        protected virtual void OnTrackGroupChanged(EntityObject entity,string preGroupName,string curGroupName)
        {
            entity.SendEvent(EntityInnerEventConst.TIMELINE_GROUP_CHANGED_ID, preGroupName,curGroupName);
        }

        protected virtual void OnTrackGroupFinish(EntityObject entity, string groupName)
        {
            entity.SendEvent(EntityInnerEventConst.TIMELINE_GROUP_FINISH_ID, groupName);
        }
    }
}
