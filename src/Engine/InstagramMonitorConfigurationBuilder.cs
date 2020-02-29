using System.Collections.Generic;
using InstaMonitor.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InstaMonitor.Engine
{
    public class InstagramMonitorConfigurationBuilder : BaseConfigurationBuilder
    {
        //public InstaMonitorConfigurationBuilder() : base(null, null)
        //{
        //}

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<InstragramEngine>();
            services.AddSingleton<DataRepository>();
        }
    }
}
