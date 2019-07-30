namespace Dot.Core.TimeLine
{
    public abstract class AEventItem : AItem
    {
        public bool CanRevert { get; set; } = true;

        public virtual void DoRevert()
        {
        }
        public abstract void Trigger();
    }
}
