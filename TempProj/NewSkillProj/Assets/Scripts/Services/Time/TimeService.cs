public class TimeService : AService
{
    public TimeService(Contexts contexts) : base(contexts)
    {
    }

    public float DeltaTime() => UnityEngine.Time.deltaTime;
}