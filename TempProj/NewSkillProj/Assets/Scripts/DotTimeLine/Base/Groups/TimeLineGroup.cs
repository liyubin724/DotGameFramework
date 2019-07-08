using DotTimeLine.Base.Condition;
using DotTimeLine.Base.Tracks;
using Entitas;
using System.Collections.Generic;

namespace DotTimeLine.Base.Groups
{
    public delegate void OnGroupFinished(TimeLineGroup group);

    public class TimeLineGroup : ATimeLineEnv
    {
        public string Name { get; set; } = "Time Line Group";
        public float TotalTime { get; set; } = 10;
        public bool IsEnd { get; set; } = false;

        public TimeLineConditionCompose conditionCompose = null;
        public readonly List<TimeLineTrack> tracks = new List<TimeLineTrack>();
        public OnGroupFinished onFinished = null;


        private float elapsedTime = 0f;
        public override void Initialize(Contexts contexts, Services services, IEntity entity)
        {
            base.Initialize(contexts, services, entity);
            tracks?.ForEach((track) =>
            {
                track?.Initialize(contexts, services, entity);
            });
            conditionCompose?.Initialize(contexts, services, entity);
        }

        public void DoUpdate(float deltaTime)
        {
            tracks?.ForEach((track) =>
            {
                track?.DoUpdate(deltaTime);
            });

            elapsedTime += deltaTime;
            if(elapsedTime>=TotalTime)
            {
                Stop();
            }
        }

        public override void DoReset()
        {
            onFinished = null;
            conditionCompose?.DoReset();
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

        public void Pause()
        {
            tracks?.ForEach((track) =>
            {
                track?.Pause();
            });
        }

        public void Resume()
        {
            tracks?.ForEach((track) =>
            {
                track?.Resume();
            });
        }
    }
}
