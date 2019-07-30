using System;

namespace Dot.Core.TimeLine
{
    public enum TimeLineExportPlatform
    {
        Client,
        Server,
        ALL,
    }

    [AttributeUsage(AttributeTargets.Class,AllowMultiple =false,Inherited =true)]
    public class TimeLineMarkAttribute : Attribute
    {
        public string Category { get; }
        public string Label { get; }
        public TimeLineExportPlatform Target{get;}
        public TimeLineMarkAttribute(string category,string label,TimeLineExportPlatform target)
        {
            Category = category;
            Label = label;
            Target = target;
        }
    }
}
