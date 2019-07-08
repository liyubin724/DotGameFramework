public class IDService : Service, IIDService
{
    private int id = 0;
    public IDService(Contexts contexts) : base(contexts)
    {
    }

    public override void DoDestroy()
    {
    }

    public override void DoReset()
    {
    }

    public int GetNext() => ++id;
}