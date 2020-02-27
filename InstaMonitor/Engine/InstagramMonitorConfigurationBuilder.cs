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
            //List<MonitorItem> monitorItems = new List<MonitorItem>();
            //Configuration.GetSection("MonitorItems").Bind(monitorItems);

            services.AddTransient<InstragramEngine>();
        }
    }
}
