using ExtractInject;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExtractInjectTest
{
    public class TestParamClass : ExtractInjectObject
    {
        public int intValue = 11;
        public int strValue = 22;

        public TestParamClass() { }

        public TestParamClass(IExtractInjectContext context) : base(context)
        {
        }
    }
}
