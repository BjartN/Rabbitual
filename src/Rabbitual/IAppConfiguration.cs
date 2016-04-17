using System.Configuration;

namespace Rabbitual
{
    public interface IAppConfiguration
    {
        string Get(string key);
    }

    public class AppConfiguration : IAppConfiguration
    {
        public string Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}