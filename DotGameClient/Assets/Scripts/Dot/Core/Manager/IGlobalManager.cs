namespace Dot.Core.Manager
{
    public interface IGlobalManager
    {
        int Priority { get; set; }
        void DoInit();
        void DoDispose();
        void DoReset();
    }
}
