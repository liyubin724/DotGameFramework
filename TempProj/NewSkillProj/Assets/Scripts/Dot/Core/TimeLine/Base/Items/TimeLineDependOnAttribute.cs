using System;

namespace DotTimeLine.Base.Items
{
    public enum TimeLineDependOnOption
    {
        Track,
        Group,
        Controller,
    }

    [AttributeUsage(AttributeTargets.Property,AllowMultiple =false)]
    public class TimeLineDependOnAttribute : Attribute
    {
        public Type DependOnType { get; }
        public TimeLineDependOnOption DependOnOption { get; }
        
        public TimeLineDependOnAttribute(Type type, TimeLineDependOnOption option = TimeLineDependOnOption.Group)
        {
            DependOnType = type;
            DependOnOption = option;
        }
    }
}
