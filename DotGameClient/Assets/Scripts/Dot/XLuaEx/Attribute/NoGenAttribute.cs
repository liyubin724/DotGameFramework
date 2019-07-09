using System;

namespace Dot.XLuaEx
{
    [AttributeUsage(AttributeTargets.Method|AttributeTargets.Field|AttributeTargets.Class|AttributeTargets.Property,AllowMultiple =false,Inherited = false)]
    public class NoGenAttribute : Attribute
    {
        public NoGenAttribute()
        {

        }
    }
}
