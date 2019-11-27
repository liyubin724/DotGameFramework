using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Text;

namespace Dot.Core.Log
{
    public static class LogUtil
    {
        private const string LOG_TAG = "LogTag";

        private static ILog logger = null;
        public static void InitWithConfigPath(string configPath)
        {
            if(File.Exists(configPath))
            {
                FileInfo fInfo = new FileInfo(configPath);
                XmlConfigurator.Configure(fInfo);

                logger = LogManager.GetLogger(typeof(LogUtil));
            }
        }

        public static void InitWithConfigBytes(byte[] configBytes)
        {
            if(configBytes!=null && configBytes.Length>0)
            {
                using(MemoryStream ms = new MemoryStream(configBytes))
                {
                    XmlConfigurator.Configure(ms);
                    logger = LogManager.GetLogger(typeof(LogUtil));
                }
            }
        }

        public static void InitWithConfig(string configText)
        {
            if(!string.IsNullOrEmpty(configText))
            {
                InitWithConfigBytes(Encoding.UTF8.GetBytes(configText));
            }
        }

        public static bool IsColorful { get; set; } = true;

        private const string MSG_FORMAT = "{0} : {1}";
        private const string COLORFUL_LOG_MSG_FORMAT = "<color=navy><b>{0} : {1}</color>";
        public static void Log(string message,string tag = LOG_TAG)
        {
            if(logger!=null)
            {
                logger.Info(string.Format(IsColorful ? COLORFUL_LOG_MSG_FORMAT : MSG_FORMAT,tag, message));
            }
        }

        private const string COLORFUL_ERROR_MSG_FORMAT = "<color=red><b>{0} : {1}</color>";
        public static void LogError(string message, string tag = LOG_TAG)
        {
            if (logger != null)
            {
                logger.Error(string.Format(IsColorful ? COLORFUL_ERROR_MSG_FORMAT : MSG_FORMAT, tag, message));
            }
        }

        private const string COLORFUL_WARN_MSG_FORMAT = "<color=orange><b>{0} : {1}</color>";
        public static void LogWarn(string message, string tag = LOG_TAG)
        {
            if (logger != null)
            {
                logger.Error(string.Format(IsColorful ? COLORFUL_WARN_MSG_FORMAT : MSG_FORMAT, tag, message));
            }
        }

        private const string COLORFUL_EXCEPTION_MSG_FORMAT = "<color=magenta><b>{0} : {1}</color>";
        public static void LogException(Exception e, string tag = LOG_TAG)
        {
            if (logger != null)
            {
                logger.Error(string.Format(IsColorful ? COLORFUL_EXCEPTION_MSG_FORMAT : MSG_FORMAT, tag, e.Message));
            }
        }
    }
}
