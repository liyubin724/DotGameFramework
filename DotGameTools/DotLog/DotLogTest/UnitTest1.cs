using NUnit.Framework;
using Dot.Core.Log;
using System.IO;
using System;

namespace DotLogTest
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
            string configPath = "E:/WorkSpace/DotGameFramework/DotGameTools/DotLog/log4net.xml";
            LogUtil.InitWithConfigPath(configPath);

            LogUtil.LogError("SSSS");


        }
    }
}