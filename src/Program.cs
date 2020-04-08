using InstaMonitor.Engine;
using InstaMonitor.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaMonitor
{
    class Program
    {
        private static ILogger commonLogger = LogManager.GetLogger(Consts.LogTypes.Common);

        static async Task Main(string[] args)
        {
            try
            {
                commonLogger.Info("InstaMonitor started");
 
                InstagramMonitorConfigurationBuilder ioc = new InstagramMonitorConfigurationBuilder();
                using (IServiceScope scope = ioc.CreateServiceScope())
                {
                    // Parse configuration. Get list of objects for monitoring
                    List<MonitorItem> monitorItems = ioc.Configuration.GetSection("MonitorItems").Get<List<MonitorItem>>();

                    if (monitorItems != null && monitorItems.Any())
                    {
                        IInstagramEngine engine = scope.ServiceProvider.GetService<IInstagramEngine>();
                        await engine.Initialize();

                        IDataRepository repo = scope.ServiceProvider.GetService<IDataRepository>();

                        ICheckExecutor checker = new CheckExecutor(engine, repo);
                        await checker.Check(monitorItems);
                    }
                }
            }
            catch (Exception e)
            {
                commonLogger.Error(e);
            }
            finally
            {
                commonLogger.Info("InstaMonitor finished");
            }
        }
    }
}
