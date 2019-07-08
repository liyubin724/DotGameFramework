public class TimeService : Service, ITimeService
{
    public TimeService(Contexts contexts) : base(contexts)
    {
    }

    public float DeltaTime() => UnityEngine.Time.deltaTime;

    public override void DoDestroy()
    {
        
    }

    public override void DoReset()
    {
        
    }
}