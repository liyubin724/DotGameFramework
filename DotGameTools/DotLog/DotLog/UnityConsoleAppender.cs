using log4net.Appender;
using log4net.Core;
using UnityEngine;

namespace Dot.Core.Log
{
    public class UnityConsoleAppender : AppenderSkeleton
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            string message = RenderLoggingEvent(loggingEvent);
            if(loggingEvent.Level == Level.Error)
            {
                Debug.LogError(message);
            }else if(loggingEvent.Level == Level.Warn)
            {
                Debug.LogWarning(message);
            }else
            {
                Debug.Log(message);
            }
        }
    }
}
