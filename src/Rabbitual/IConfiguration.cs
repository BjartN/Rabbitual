using System.Configuration;

namespace Rabbitual
{
    public interface IConfiguration
    {
        string Get(string key);
    }

    public class Configuration: IConfiguration
    {
        public string Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}