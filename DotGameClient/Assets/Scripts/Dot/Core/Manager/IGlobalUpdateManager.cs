namespace Dot.Core.Manager
{
    public interface IGlobalUpdateManager : IGlobalManager
    {
        void DoUpdate(float deltaTime);
    }
}
