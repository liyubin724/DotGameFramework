using System;
using System.Collections.Generic;
using System.Text;
using ExtractInject;

namespace ExtractInjectTest
{
    public class TestClass : UsedExtractInject
    {
        [ExtractInjectField]
        public TestParamClass paramClass;

        public bool Run()
        {
            return paramClass.intValue == 11;
        }
    }
}
