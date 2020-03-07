using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.IO;

namespace InstaMonitor.Engine
{
    /// <summary>
    /// Base builder of application configuration
    /// </summary>
    public abstract class BaseConfigurationBuilder
    {
        private static ILogger commonlogger = LogManager.GetLogger(Consts.LogTypes.Common);
        public IConfiguration Configuration { get; private set; }
        protected IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// Name of environment variable with environment name
        /// </summary>
        protected readonly string EnvironmentVariableName = "INSTAMONITOR_ENVIRONMENT";

        /// <summary>
        /// ConfigurationBuilder's constructor
        /// if envVariableName is empty then will be used default Environment Variable Name (INSTAMONITOR_ENVIRONMENT)
        /// EnvironmentName source priority:
        /// 1. Value from file envsettings.json
        /// 2. Value of envName passed to the constructor
        /// 3. Value of environment variable. Name of variable is INSTAMONITOR_ENVIRONMENT (by default) or envVariableName (if passed)
        /// </summary>
        /// <param name="envName"></param>
        /// <param name="envVariableName"></param>
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
            commonlogger.Debug($"EnviromentName = '{envName}'");
            Configuration = BuildConfiguration(envName);
            ServiceProvider = BuildServiceProvider(Configuration);
        }

        /// <summary>
        /// Create scope of IoC container. Use it for avoid reference leak
        /// </summary>
        /// <returns></returns>
        public IServiceScope CreateServiceScope()
        {
            return ServiceProvider.CreateScope();
        }

        /// <summary>
        /// Abstract method for configure dependency injection (IoC)
        /// </summary>
        /// <param name="services"></param>
        protected abstract void ConfigureServices(IServiceCollection services);

        /// <summary>
        /// Read configuration files and build application configuration
        /// </summary>
        /// <param name="envName"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Get value of environment name for select configuration file
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Create container
        /// </summary>
        /// <returns></returns>
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
