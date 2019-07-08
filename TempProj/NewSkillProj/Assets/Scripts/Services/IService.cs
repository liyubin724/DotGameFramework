public interface IService
{
    Contexts CachedContexts { get; }
    void DoReset();
    void DoDestroy();
}