using System.Collections.Generic;
using InstaMonitor.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InstaMonitor.Engine
{
    /// <summary>
    /// Implementaion of ConfigurationBuilder for Instragram Monitor
    /// </summary>
    public class InstagramMonitorConfigurationBuilder : BaseConfigurationBuilder
    {
        //public InstaMonitorConfigurationBuilder() : base(null, null)
        //{
        //}

        /// <summary>
        /// Configure dependency injection container
        /// </summary>
        /// <param name="services"></param>
        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IInstagramEngine, InstagramEngine>();
            services.AddSingleton<IDataRepository, DataRepository>();
        }
    }
}
