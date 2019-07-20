namespace Dot.Core.TimeLine
{
    public abstract class AActionItem : AItem
    {
        protected float duration = 1.0f;
        public float Duration
        {
            get { return duration; }
            set
            {
                duration = value;
                if(duration <0)
                {
                    duration = 0;
                }
            }
        }

        public float EndTime
        {
            get { return FireTime + Duration; }
        }

        public abstract void Enter();
        public virtual void DoUpdate(float deltaTime)
        {
        }
        public abstract void Exit();
        
        public virtual void Stop()
        {
        }

        public virtual void Pause()
        {
        }

        public virtual void Resume()
        {
        }
    }
}
