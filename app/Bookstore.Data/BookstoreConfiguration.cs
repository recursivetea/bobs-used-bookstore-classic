using System;
using System.Collections.Generic;

namespace BobsBookstoreClassic.Data
{
    /// <summary>
    /// Provides a runtime-configurable settings store that can be seeded from
    /// IConfiguration (appsettings.json / environment variables) and augmented
    /// at runtime (e.g. from AWS SSM Parameter Store).
    /// </summary>
    public sealed class BookstoreConfiguration
    {
        private static readonly Lazy<BookstoreConfiguration> Lazy =
            new Lazy<BookstoreConfiguration>(() => new BookstoreConfiguration());

        private static BookstoreConfiguration Instance => Lazy.Value;

        private readonly Dictionary<string, string> _appSettings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> _connectionStrings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private BookstoreConfiguration() { }

        /// <summary>Called once at startup to seed settings from IConfiguration.</summary>
        public static void Initialize(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            foreach (var kvp in Microsoft.Extensions.Configuration.ConfigurationExtensions.AsEnumerable(configuration))
            {
                if (kvp.Value != null)
                    Instance._appSettings[kvp.Key] = kvp.Value;
            }

            foreach (var cs in configuration.GetSection("ConnectionStrings").GetChildren())
            {
                Instance._connectionStrings[cs.Key] = cs.Value ?? string.Empty;
            }
        }

        public static void AddSetting(string key, string value)
        {
            Instance._appSettings[key] = value;
        }

        public static string GetSetting(string key)
        {
            Instance._appSettings.TryGetValue(key, out var value);
            return value ?? string.Empty;
        }

        public static T GetSetting<T>(string key)
        {
            var value = GetSetting(key);
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public static void AddConnectionString(string key, string value)
        {
            Instance._connectionStrings[key] = value;
        }

        public static string GetConnectionString(string key)
        {
            Instance._connectionStrings.TryGetValue(key, out var value);
            return value ?? string.Empty;
        }
    }
}
