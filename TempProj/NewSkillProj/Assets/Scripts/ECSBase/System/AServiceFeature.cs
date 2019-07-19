public abstract class AServiceFeature : Feature
{
    protected Contexts contexts;
    protected Services services;

    public AServiceFeature(string name, Contexts contexts, Services services) : base(name)
    {
        this.contexts = contexts;
        this.services = services;
    }
}