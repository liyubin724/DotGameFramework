using System;

namespace ExtractInject
{
    public enum ExtractInjectUsage
    {
        InOut,
        In,
        Out,
    }

    [AttributeUsage(AttributeTargets.Field,AllowMultiple =false)]
    public class ExtractInjectField : Attribute
    {
        public ExtractInjectUsage Usage { get; set; }
        public bool IsOptional { get; set; }

        public ExtractInjectField(ExtractInjectUsage usage = ExtractInjectUsage.InOut,bool optional = false)
        {
            Usage = usage;
            IsOptional = optional;
        }
    }
}
