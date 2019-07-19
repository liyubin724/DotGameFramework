public partial class Services
{
    public TimeService timeService;
    public ILogService logService;
    public UniqueIDService idService;
    public EntityFactroy entityFactroy;
    public ConfigDataService dataService;

    public Services(Contexts contexts)
    {
        logService = new UnityLogService(contexts);
        timeService = new TimeService(contexts);
        entityFactroy = new EntityFactroy(contexts, this);
        dataService = new ConfigDataService(contexts);
        idService = new UniqueIDService(contexts);
    }

    public void DoDispose()
    {
        (logService as AService).DoDispose();
    }
    
}