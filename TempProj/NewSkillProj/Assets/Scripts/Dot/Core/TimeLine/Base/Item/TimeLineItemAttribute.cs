using System;

namespace Dot.Core.TimeLine.Base.Item
{
    public enum TimeLineItemPlatform
    {
        Client,
        Server,
        ALL,
    }

    [AttributeUsage(AttributeTargets.Class,AllowMultiple =false,Inherited =true)]
    public class TimeLineItemAttribute : Attribute
    {
        public string Category { get; }
        public string Label { get; }
        public TimeLineItemPlatform Target{get;}
        public TimeLineItemAttribute(string category,string label,TimeLineItemPlatform target)
        {
            Category = category;
            Label = label;
            Target = target;
        }
    }
}
