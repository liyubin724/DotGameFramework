using Dot.Core.TimeLine.Base.Condition;
using Dot.Core.TimeLine.Base.Tracks;
using Entitas;
using System.Collections.Generic;

namespace Dot.Core.TimeLine.Base.Groups
{
    public delegate void OnGroupFinished(TimeLineGroup group);

    public class TimeLineGroup : ATimeLineEnv
    {
        public string Name { get; set; } = "Time Line Group";
        public float TotalTime { get; set; } = 10;
        public bool IsAwaysRun { get; set; } = false;

        public TimeLineParallelCondition beginParallel = new TimeLineParallelCondition();
        public ATimeLineComposeCondition endCompose = null;

        public readonly List<TimeLineTrack> tracks = new List<TimeLineTrack>();
        public OnGroupFinished onFinished = null;
        
        public override void Initialize(Contexts contexts, Services services, IEntity entity)
        {
            base.Initialize(contexts, services, entity);
            tracks?.ForEach((track) =>
            {
                track?.Initialize(contexts, services, entity);
            });
            beginParallel?.Initialize(contexts, services, entity);
            endCompose?.Initialize(contexts, services, entity);
        }

        public void DoUpdate(float deltaTime)
        {
            if(endCompose!=null)
            {
                endCompose.DoUpdate(deltaTime);
                if(endCompose.Evaluate())
                {
                    Stop();
                    return;
                }
            }

            tracks?.ForEach((track) =>
            {
                track?.DoUpdate(deltaTime);
            });
        }

        public override void DoReset()
        {
            onFinished = null;

            beginParallel?.DoReset();
            endCompose?.DoReset();

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
