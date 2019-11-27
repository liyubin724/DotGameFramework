namespace ExtractInject
{
    public class UsedExtractInject : IUsedExtractInject
    {
        public void Extract(IExtractInjectContext context)
        {
            ExtractInjectUtil.Extract(context, this);
        }

        public void Inject(IExtractInjectContext context)
        {
            ExtractInjectUtil.Inject(context, this);
        }
    }
}
