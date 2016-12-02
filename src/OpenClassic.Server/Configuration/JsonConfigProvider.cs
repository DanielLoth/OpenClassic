using Microsoft.Extensions.Configuration;
using System.IO;

namespace OpenClassic.Server.Configuration
{
    public class JsonConfigProvider : IConfigProvider
    {
        public IConfig GetConfig()
        {
            var configBuilder = new ConfigurationBuilder()
                //.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Settings.json")
                .Build();

            var config = new Config();

            configBuilder.Bind(config);

            return config;
        }
    }
}
