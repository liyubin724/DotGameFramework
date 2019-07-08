public abstract class ServiceFeature : Feature
{
    protected Contexts contexts;
    protected Services services;

    public ServiceFeature(string name,Contexts contexts,Services services):base(name)
    {
        this.contexts = contexts;
        this.services = services;
    }
}