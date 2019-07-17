using Dot.Core.TimeLine.Base.Condition;
using Dot.Core.TimeLine.Base.Tracks;
using Entitas;
using System.Collections.Generic;

namespace Dot.Core.TimeLine.Base.Group
{
    public delegate void OnGroupFinished(TrackGroup group);

    public class TrackGroup : AEntitasEnv
    {
        public string Name { get; set; } = "Time Line Group";
        private float totalTime = 10.0f;
        public float TotalTime
        {
            get
            {
                return totalTime;
            }
            set
            {
                totalTime = value;
                if(endCondition == null)
                {
                    endCondition = new ParallelCondition();
                    endCondition.IsReadonly = true;
                    TimeOverCondition toCondition = new TimeOverCondition();
                    toCondition.IsReadonly = true;
                    toCondition.TotalTime = TotalTime;
                    endCondition.conditions.Add(toCondition);
                }
                foreach(var c in endCondition.conditions)
                {
                    if(c.GetType() == typeof(TimeOverCondition))
                    {
                        ((TimeOverCondition)c).TotalTime = totalTime;
                        break;
                    }
                }
            }
        }
        public bool IsAwaysRun { get; set; } = false;

        public ACondition beginCondition = null;
        public ParallelCondition endCondition = null;

        public readonly List<TrackLine> tracks = new List<TrackLine>();
        public OnGroupFinished onFinished = null;
        
        public override void Initialize(Contexts contexts, Services services, IEntity entity)
        {
            base.Initialize(contexts, services, entity);
            tracks?.ForEach((track) =>
            {
                track?.Initialize(contexts, services, entity);
            });
            beginCondition?.Initialize(contexts, services, entity);
            endCondition?.Initialize(contexts, services, entity);
        }

        public void DoUpdate(float deltaTime)
        {
            tracks?.ForEach((track) =>
            {
                track?.DoUpdate(deltaTime);
            });

            if (endCondition != null)
            {
                endCondition.DoUpdate(deltaTime);
                if (endCondition.Evaluate())
                {
                    Stop();
                    return;
                }
            }
        }

        public override void DoReset()
        {
            onFinished = null;

            beginCondition?.DoReset();
            endCondition?.DoReset();

            base.DoReset();
        }

        public void Stop()
        {
            tracks?.ForEach((track) =>
            {
                track?.Stop();
            });
            onFinished?.Invoke(this);
        }


        public static TrackGroup CreateNew()
        {
            TrackGroup group = new TrackGroup();
            group.endCondition = new ParallelCondition();
            group.endCondition.IsReadonly = true;
            TimeOverCondition toCondition = new TimeOverCondition();
            toCondition.IsReadonly = true;
            toCondition.TotalTime = group.TotalTime;
            group.endCondition.conditions.Add(toCondition);
            return group;
        }

        //public void Pause()
        //{
        //    tracks?.ForEach((track) =>
        //    {
        //        track?.Pause();
        //    });
        //}

        //public void Resume()
        //{
        //    tracks?.ForEach((track) =>
        //    {
        //        track?.Resume();
        //    });
        //}
    }
}
