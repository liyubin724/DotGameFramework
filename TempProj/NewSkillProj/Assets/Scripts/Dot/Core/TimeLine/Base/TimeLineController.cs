using Dot.Core.TimeLine.Base.Condition;
using Dot.Core.TimeLine.Base.Group;
using Entitas;
using System.Collections.Generic;

namespace Dot.Core.TimeLine.Base
{
    public class TimeLineController : AEntitasEnv
    {
        public List<TrackGroup> groups = new List<TrackGroup>();
        public TimeLineState State { get; set; } = TimeLineState.Invalid;

        private int runningIndex = -1;
        public override void Initialize(Contexts contexts, Services services, IEntity entity)
        {
            base.Initialize(contexts, services, entity);
            groups.ForEach((group) =>
            {
                group.Initialize(contexts, services, entity);
                group.onFinished += OnGroupFinished;
            });
        }

        public override void DoReset()
        {
            if(State!= TimeLineState.Finished && State!= TimeLineState.Invalid)
            {
                services.logService.Log(DebugLogType.Error, "");
                return;
            }
            runningIndex = -1;
            State = TimeLineState.Invalid;
            base.DoReset();
        }

        public void Play()
        {
            if(State == TimeLineState.Invalid)
            {
                runningIndex = 0;
                State = TimeLineState.Waiting;
            }
        }

        public void Stop()
        {
            if(State!= TimeLineState.Running || State!= TimeLineState.Waiting)
            {
                return;
            }
            if(State == TimeLineState.Running)
            {
                if(!groups[runningIndex].IsAwaysRun)
                {
                    groups[runningIndex].onFinished = null;
                    groups[runningIndex].Stop();
                    groups[runningIndex].DoReset();
                }else
                {
                    return;
                }
            }
            runningIndex = -1;
            for (int i = runningIndex + 1; i < groups.Count; ++i)
            {
                if (groups[i].IsAwaysRun)
                {
                    runningIndex = i;
                    State = TimeLineState.Running;
                    break;
                }
            }
            if(runningIndex<0)
            {
                State = TimeLineState.Finished;
            }
        }

        private void OnGroupFinished(TrackGroup group)
        {
            if(State == TimeLineState.Running)
            {
                group.DoReset();
                ++runningIndex;
                if(runningIndex>=groups.Count)
                {
                    State = TimeLineState.Finished;
                }else
                {
                    State = TimeLineState.Waiting;
                }
            }
        }

        public void DoUpdate(float deltaTime)
        {
            if(State == TimeLineState.Invalid || State == TimeLineState.Finished)
            {
                return;
            }
            if(State == TimeLineState.Waiting)
            {
                if(runningIndex>=0&&runningIndex<groups.Count)
                {
                    AComposeCondition condition = groups[runningIndex].beginParallel;
                    condition?.DoUpdate(deltaTime);
                    if (condition == null || condition.Evaluate())
                    {
                        State = TimeLineState.Running;
                        groups[runningIndex].Initialize(contexts, services, entity);
                    }else
                    {
                        return;
                    }
                }else
                {
                    return;
                }
            }
            if(State == TimeLineState.Running)
            {
                if (runningIndex >= 0 && runningIndex < groups.Count)
                {
                    groups[runningIndex].DoUpdate(deltaTime);
                }
            }
        }
    }
}
