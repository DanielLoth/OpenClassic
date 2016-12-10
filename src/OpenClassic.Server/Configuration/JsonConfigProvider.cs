using Microsoft.Extensions.Configuration;

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
            config.Validate();

            return config;
        }
    }
}
