namespace ExtractInject
{
    public interface IUsedExtractInject
    {
        void Inject(IExtractInjectContext context);
        void Extract(IExtractInjectContext context);
    }
}
