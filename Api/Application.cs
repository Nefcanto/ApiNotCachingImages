using Microsoft.Extensions.Hosting;

namespace Api;

public class Application
{
    private static Dictionary<int, Action> initializers = new Dictionary<int, Action>();

    public static void RegisterInitializer(Action initializer)
    {
        var hash = initializer.GetHashCode();
        if (initializers.ContainsKey(hash))
        {
            return;
        }
        initializers.Add(hash, initializer);
    }

    public static void Run()
    {
        try
        {
            FindAndRunConfigurations();
            foreach (var initializer in initializers)
            {
                initializer.Value.Invoke();
            }
            var builder = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                .UseStartup<Startup>()
                .UseUrls("http://0.0.0.0:5000");
            });
            builder.Build().Run();
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
        }
    }

    private static void FindAndRunConfigurations()
    {
        var configs = typeof(Application)
        .Assembly
        .GetTypes()
        .Where(i => i.Name.EndsWith("Config"))
        .ToList();
        foreach (var config in configs)
        {
            var configureMethod = config.GetMethod("Configure");
            if (configureMethod != null)
            {
                Logger.LogInfo($"Running {configureMethod.DeclaringType.FullName}");
                configureMethod.Invoke(null, null);
            }
        }
    }
}
