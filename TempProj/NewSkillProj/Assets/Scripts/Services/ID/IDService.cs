public class UniqueIDService : AService
{
    private int id = 0;
    public UniqueIDService(Contexts contexts) : base(contexts)
    {
    }

    public int GetNext() => ++id;
}