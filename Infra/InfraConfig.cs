namespace Infra;

public class InfraConfig
{
    static IConfigurationRoot ConfigurationRoot { get; set; }

    private static string logFolderPath;

    static InfraConfig()
    {
        InitializeConfiguration();
    }

    public static bool IsSuperAdmin
    {
        get
        {
            var isSuperAdmin = HttpContextHelper.Current.Items["IsSuperAdmin"];
            return isSuperAdmin != null && isSuperAdmin.ToBoolean() == true;
        }
    }

    public static ParallelOptions ParallelOptions
    {
        get
        {
            var options = new ParallelOptions();
            options.MaxDegreeOfParallelism = Environment.ProcessorCount;
            return options;
        }
    }

    public static string LogFolderPath
    {
        get
        {
            if (logFolderPath.IsSomething())
            {
                return logFolderPath;
            }
            logFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            return logFolderPath;
        }
    }

    public static string GetSetting(string key)
    {
        string result = ConfigurationRoot[key];
        if (result.IsNothing())
        {
            throw new ServerException($"There is no setting for {key} in Settings.json file, or in SettingsOverride.json file, or LocalSecrects.json, or these files are not present at the base directory of execution path, Or the config has no value.");
        }
        return result;
    }

    public static string Debug()
    {
        return ConfigurationRoot.GetDebugView();
    }

    private static void InitializeConfiguration()
    {
        if (ConfigurationRoot == null)
        {
            try
            {
                ConfigurationRoot = new ConfigurationBuilder()
                    .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.json"), true)
                    .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SettingsOverride.json"), true)
                    .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LocalSecrets.json"), true)
                    .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConnectionStrings.json"), true)
                    .Build();
            }
            catch (Exception)
            {
                throw new ServerException("Contents of Settings.json or SettingsOverride.json or ConnectionStrings.json are not valid JSON content. Check them.");
            }
        }
    }

    public static bool HasSetting(string key)
    {
        return ConfigurationRoot[key] != null;
    }

    public static bool IsDeveloping
    {
        get
        {
            var key = "IsDeveloping";
            if (!HasSetting(key))
            {
                return false;
            }
            var isDeveloping = GetSetting(key).ToBoolean();
            return isDeveloping;
        }
    }

    public static int DefaultPageSize
    {
        get
        {
            if (HasSetting("DefaultPageSize"))
            {
                string pageSize = GetSetting("DefaultPageSize");
                if (pageSize.IsNumeric())
                {
                    return Convert.ToInt32(pageSize);
                }
            }
            return 10;
        }
    }

    public static bool HasConnectionString(string name)
    {
        var connectionString = ConfigurationRoot[name];
        if (connectionString == null)
        {
            return false;
        }
        return true;
    }

    public static string GetConnectionString(string name)
    {
        var connectionString = ConfigurationRoot[name];
        if (connectionString == null)
        {
            if (HasSetting("DatabaseServer") && HasSetting("DatabaseUser") && HasSetting("DatabasePassword"))
            {
                connectionString = $"server={GetSetting("DatabaseServer")};{(HasSetting("DatabasePort") ? $" port={GetSetting("DatabasePort")};" : "")} user id={GetSetting("DatabaseUser")}; password={GetSetting("DatabasePassword")}; database={name};";
            }
            else
            {
                throw new ServerException($"No connection string with name {name} can be found. Please have a ConnectionStrings.json file and make it copy to output always, and make sure connection string key is present in it. Or make sure your Settings.json file contains DatabaseServer, DatabaseUser and DatabasePassword.");
            }
        }
        return connectionString;
    }

    public static bool ResizeAndConvertImages
    {
        get
        {
            var key = nameof(ResizeAndConvertImages);
            if (HasSetting(key))
            {
                return GetSetting(key).ToBoolean();
            }
            return false;
        }
    }

    public static bool HasEnvironmentVariable(string key)
    {
        var result = Environment.GetEnvironmentVariable(key);
        if (result.IsNothing())
        {
            return false;
        }
        return true;
    }

    public static string GetEnvironmentVariable(string key)
    {
        return GetEnvironmentVariable(key, null);
    }

    public static string GetEnvironmentVariable(string key, string alternative)
    {
        var result = Environment.GetEnvironmentVariable(key);
        if (result.IsNothing())
        {
            result = alternative;
        }
        if (result.IsNothing())
        {
            throw new ServerException($"{key} should be defined in Environment Variables, or a fallback should be provided.");
        }
        return result;
    }
}
