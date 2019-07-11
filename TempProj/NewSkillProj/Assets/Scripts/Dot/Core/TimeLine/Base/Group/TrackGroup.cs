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
        public float TotalTime { get; set; } = 10;
        public bool IsAwaysRun { get; set; } = false;

        public AComposeCondition beginCondition = null;
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
            if(endCondition!=null)
            {
                endCondition.DoUpdate(deltaTime);
                if(endCondition.Evaluate())
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
