namespace Dot.Core.TimeLine
{
    public abstract class AEventItem : AItem
    {
        public bool CanRevert { get; set; } = true;

        public abstract void DoRevert();
        public abstract void Trigger();
    }
}
