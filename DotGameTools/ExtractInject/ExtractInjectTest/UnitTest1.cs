using ExtractInject;
using ExtractInjectTest;
using NUnit.Framework;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            ExtractInjectContext context = new ExtractInjectContext();
            _ = new TestParamClass(context);

            TestClass tClass = new TestClass();
            tClass.Inject(context);

            Assert.IsTrue(tClass.Run());
        }
    }
}