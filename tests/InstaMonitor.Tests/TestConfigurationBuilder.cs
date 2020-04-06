using System.Collections.Generic;
using InstaMonitor.Engine;
using InstaMonitor.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InstaMonitor.Tests
{
    /// <summary>
    /// Implementaion of ConfigurationBuilder for Instragram Monitor
    /// </summary>
    public class TestConfigurationBuilder : BaseConfigurationBuilder
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
            // services.AddTransient<InstragramEngine>();
            // services.AddSingleton<IDataRepository, DataRepository>();
        }
    }
}
