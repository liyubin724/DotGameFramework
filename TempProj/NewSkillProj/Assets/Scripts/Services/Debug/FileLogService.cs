using System.IO;
using System.Text;

public class FileLogService : Service, ILogService
{
    StreamWriter writer = null;
    public FileLogService(Contexts contexts,string filePath) : base(contexts)
    {
        writer = new StreamWriter(filePath, true, Encoding.UTF8);
    }

    public override void DoDestroy()
    {
        if(writer!=null)
        {
            writer.Close();
        }
    }

    public override void DoReset()
    {
    }

    public void Log(DebugLogType logType, string message)
    {
        switch (logType)
        {
            case DebugLogType.Error:
                writer?.WriteLine("Error::" + message);
                break;
            case DebugLogType.Info:
                writer?.WriteLine("Log::" + message);
                break;
            case DebugLogType.Warning:
                writer?.WriteLine("Warning::" + message);
                break;
            case DebugLogType.Exception:
                writer?.WriteLine("Exception::" + message);
                break;
        }
        writer?.Flush();
    }
}