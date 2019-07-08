using System;

namespace DotTimeLine.Items
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple =false)]
    public class TimeLineDependOnAttribute : Attribute
    {
    }
}
