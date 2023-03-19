namespace Infra;

public static class Logger
{
    static object lockToken = new object();

    public static Action<dynamic, MessageType> Persistor;

    public static void Log(dynamic @object, MessageType type)
    {
        if (@object is string)
        {
            @object.Insert(0, $"{type}: ");
        }
        if (@object is string)
        {
            Console.WriteLine("{0} - {1}", UniversalDateTime.Now.ToString("HH:mm:ss"), @object);
        }
        else
        {
            Console.WriteLine($"{type} - {@object.Serialize()}");
        }
        Console.ForegroundColor = ConsoleColor.White;
        try
        {
            LogToFile(type, @object);
        }
        catch (Exception ex)
        {
            throw new ServerException(ex.ToString());
        }
        if (Persistor != null)
        {
            new Thread(() =>
            {
                try
                {
                    Persistor(@object, type);
                }
                catch (Exception ex)
                {
                    var message = ExceptionHelper.BuildExceptionStack(ex).Merge();
                    System.Console.WriteLine("Error happened during persisting logs.");
                    System.Console.WriteLine(ex);
                    // intentionally swallowed
                }
            }).Start();
        }
    }

    internal static void LogToFile(MessageType type, dynamic @object)
    {
        var text = CreateLogEntry(type, @object);
        string logPath;
        lock (lockToken)
        {
            logPath = FindLogPath(type);
            File.AppendAllText(logPath, text, Encoding.UTF8);
        }
    }

    private static dynamic CreateLogEntry(MessageType type, dynamic @object)
    {
        return string.Format("\r\n{0}-{1}: {2}", UniversalDateTime.Now, type.ToString(), @object);
    }

    public static string FindLogPath(MessageType messageType)
    {
        string logPath = string.Format(Path.Combine(InfraConfig.LogFolderPath, $"{UniversalDateTime.Now.ToString("yyyy-MM-dd")}-{messageType.ToString()}.txt"));
        CreateDirectoryIfNotExist(logPath);
        return logPath;
    }

    private static void CreateDirectoryIfNotExist(string logPath)
    {
        if (!Directory.Exists(Path.GetDirectoryName(logPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(logPath));
        }
    }

    public static void LogException(this Exception ex)
    {
        string error = ex.Message;
        error += "\n" + ExceptionHelper.BuildExceptionStack(ex).Merge();
        LogError(error);
    }

    public static void LogError(this string text)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Log(text, MessageType.Error);
    }

    public static void LogInfo(this string text)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Log(text, MessageType.Info);
    }

    public static void LogInfo(dynamic obj)
    {
        Log(obj, MessageType.Info);
    }

    public static void LogSuccess(this string text)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Log(text, MessageType.Success);
    }

    public static void LogWarning(this string text)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Log(text, MessageType.Warning);
    }

    public static void Count(int number)
    {
        Console.Write("\r                 ");
        Console.Write("\r" + number);
    }
}
