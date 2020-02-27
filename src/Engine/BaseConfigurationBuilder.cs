using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace InstaMonitor.Engine
{
    public abstract class BaseConfigurationBuilder
    {
        public IConfiguration Configuration { get; private set; }
        public IServiceProvider ServiceProvider { get; private set; }

        protected readonly string EnvironmentVariableName = "INSTAMONITOR_ENVIRONMENT";

        public BaseConfigurationBuilder(string envName = null, string envVariableName = null)
        {
            if (!String.IsNullOrEmpty(envVariableName))
            {
                EnvironmentVariableName = envVariableName;
            }
            string envNamePriority = GetEnvironmentName();
            if (!String.IsNullOrEmpty(envNamePriority))
            {
                envName = envNamePriority;
            }
            if (String.IsNullOrEmpty(envName))
            {
                envName = Environment.GetEnvironmentVariable(EnvironmentVariableName);
            }
            Configuration = BuildConfiguration(envName);
            ServiceProvider = BuildServiceProvider(Configuration);
        }

        protected abstract void ConfigureServices(IServiceCollection services);

        protected virtual IConfiguration BuildConfiguration(string envName)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            if (!String.IsNullOrEmpty(envName))
            {
                string cfgName = $"appsettings.{envName}.json";
                builder.AddJsonFile(cfgName, optional: true, reloadOnChange: true);
            }

            return builder.Build();
        }



        private string GetEnvironmentName()
        {
            var currentDirectoryPath = Directory.GetCurrentDirectory();
            var envSettingsPath = Path.Combine(currentDirectoryPath, "envsettings.json");
            string enviromentName = null;
            if (File.Exists(envSettingsPath))
            {
                JObject envSettings = JObject.Parse(File.ReadAllText(envSettingsPath));
                if (envSettings.TryGetValue(EnvironmentVariableName, out JToken envValue))
                {
                    enviromentName = envValue?.ToString();
                }
            }
            return enviromentName;
        }


        protected virtual IServiceCollection CreateServices()
        {
            IServiceCollection services = new ServiceCollection();
            return services;
        }


        /// <summary>
        /// Configure the dependency injection services
        /// </summary>
        protected virtual IServiceProvider BuildServiceProvider(IConfiguration config)
        {
            IServiceCollection services = CreateServices();
            services.AddSingleton<IConfiguration>(config);

            ConfigureServices(services);

            IServiceProvider provider = BuildServiceProvider(services);
            return provider;
        }

        protected virtual IServiceProvider BuildServiceProvider(IServiceCollection services)
        {
            return services.BuildServiceProvider(false);
        }
    }
}
