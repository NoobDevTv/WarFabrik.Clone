using Microsoft.Extensions.Configuration;

namespace BotMaster.Core.Configuration;

public static class ConfigManager
{
    private static readonly Dictionary<string, IConfiguration> configurationsCache = new();

    private static readonly Dictionary<string, ISetting> bindedConfigurations = new();

    public static IConfiguration GetConfiguration(string configPath, params string[] args)
    {
        if (configurationsCache.TryGetValue(configPath, out var config))
            return config;

        return configurationsCache[configPath] = new ConfigurationBuilder()
          .AddJsonFile(configPath, optional: true, reloadOnChange: true)
          .AddEnvironmentVariables()
          .AddCommandLine(args)
          .Build();
    }

    public static T GetSettings<T>(this IConfiguration config) where T : ISetting, new()
    {
        var t = new T();
        if (bindedConfigurations.TryGetValue(t.ConfigName, out var setting))
            return (T)setting;

        config.GetSection(t.ConfigName).Bind(t);
        bindedConfigurations[t.ConfigName] = t;
        return t;
    }
}
