using System;
using UnityEngine;

public sealed class UnityLogService : AService, ILogService 
{
    public UnityLogService(Contexts contexts):base(contexts)
    {
    }

    public void Log(DebugLogType logType, string message)
    {
        switch(logType)
        {
            case DebugLogType.Error:
                Debug.LogError(message);
                break;
            case DebugLogType.Info:
                Debug.Log(message);
                break;
            case DebugLogType.Warning:
                Debug.LogWarning(message);
                break;
            case DebugLogType.Exception:
                Debug.LogException(new Exception(message));
                break;
        }
    }

}