﻿namespace ExtractInject
{
    public abstract class ExtractInjectObject : IExtractInjectObject
    {
        public ExtractInjectObject()
        { }

        public ExtractInjectObject(IExtractInjectContext context)
        {
            AddToContext(context);
        }

        public void AddToContext(IExtractInjectContext context)
        {
            if (context != null)
            {
                context.AddObject(this.GetType(),this);
            }
        }

        public void RemoveFromContext(IExtractInjectContext context)
        {
            if (context != null)
            {
                context.DeleteObject(this.GetType());
            }
        }
    }
}
