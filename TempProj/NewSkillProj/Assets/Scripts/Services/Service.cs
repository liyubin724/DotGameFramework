public abstract class Service : IService
{
    private readonly Contexts contexts;
    public Contexts CachedContexts => contexts;

    public Service(Contexts contexts)
    {
        this.contexts = contexts;
    }

    public abstract void DoDestroy();

    public abstract void DoReset();
}