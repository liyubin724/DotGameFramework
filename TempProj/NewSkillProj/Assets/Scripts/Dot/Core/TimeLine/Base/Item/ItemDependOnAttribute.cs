using System;

namespace Dot.Core.TimeLine.Base.Item
{
    public enum DependOnOption
    {
        Track,
        Group,
        Controller,
    }

    [AttributeUsage(AttributeTargets.Property,AllowMultiple =false)]
    public class ItemDependOnAttribute : Attribute
    {
        public Type DependOnType { get; }
        public DependOnOption DependOnOption { get; }
        
        public ItemDependOnAttribute(Type type, DependOnOption option = DependOnOption.Controller)
        {
            DependOnType = type;
            DependOnOption = option;
        }
    }
}
