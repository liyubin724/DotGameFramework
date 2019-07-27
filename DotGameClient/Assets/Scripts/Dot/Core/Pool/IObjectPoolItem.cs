namespace Dot.Core.Pool
{
    public interface IObjectPoolItem
    {
        void OnNew();
        void OnRelease();
    }
}
