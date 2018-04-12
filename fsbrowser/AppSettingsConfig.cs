
using System.Configuration;

namespace FSBrowser
{
    /// <summary>
    /// Configuration implementation that pulls values from appSettings.
    /// </summary>
    public class AppSettingsConfig : IConfig
    {
        public string HomeDirectory => ConfigurationManager.AppSettings[nameof(HomeDirectory)];
    }
}
