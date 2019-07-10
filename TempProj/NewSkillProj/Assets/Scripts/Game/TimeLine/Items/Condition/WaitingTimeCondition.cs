using Dot.Core.TimeLine.Base.Condition;

namespace Game.TimeLine
{
    public class WaitingTimeCondition : ATimeLineCondition
    {
        public float Time { get; set; } = 0.0f;

        private float elapsedTime = 0.0f;
        public override bool Evaluate()
        {
            if (elapsedTime >= Time)
                return true;

            return false;
        }

        public override void DoUpdate(float deltaTime)
        {
            elapsedTime += deltaTime;
        }
    }
}
