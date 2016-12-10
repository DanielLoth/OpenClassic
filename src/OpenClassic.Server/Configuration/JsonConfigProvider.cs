using Microsoft.Extensions.Configuration;

namespace OpenClassic.Server.Configuration
{
    public class JsonConfigProvider : IConfigProvider
    {
        private IConfig cachedConfig;

        public IConfig GetConfig()
        {
            if (cachedConfig != null)
            {
                return cachedConfig;
            }

            var configBuilder = new ConfigurationBuilder()
                //.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Settings.json")
                .Build();

            var config = new Config();
            configBuilder.Bind(config);
            config.Validate();

            cachedConfig = config;

            return config;
        }
    }
}
