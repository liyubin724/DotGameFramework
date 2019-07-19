public abstract class AService
{
    protected Contexts contexts;
    public AService(Contexts contexts)
    {
        this.contexts = contexts;
    }

    public virtual void DoDispose() { }
}