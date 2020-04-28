using System.Collections.Generic;
using InstaMonitor.Core.Engine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InstaMonitor
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
            // services.AddSingleton<ICheckExecutor, CheckExecutor>();
            services.AddTransient<IReportService, ReportService>();
            services.AddTransient<IInstagramEngine, InstagramEngine>();
            services.AddSingleton<IDataRepository, DataRepository>();
        }
    }
}
