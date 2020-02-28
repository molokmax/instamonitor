using InstaMonitor.Engine;
using InstaMonitor.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InstaMonitor
{
    class Program
    {
        

        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Begin");

                InstagramMonitorConfigurationBuilder ioc = new InstagramMonitorConfigurationBuilder();
                List<MonitorItem> monitorItems = ioc.Configuration.GetSection("MonitorItems").Get<List<MonitorItem>>();

                if (monitorItems.Any())
                {
                    InstragramEngine engine = ioc.ServiceProvider.GetService<InstragramEngine>();
                    await engine.Initialize();
                    
                    foreach (MonitorItem item in monitorItems)
                    {
                        if (item.Followers)
                        {
                            List<string> followers = await engine.GetFollowers(item.UserName);

                            Console.WriteLine($"{item.UserName}'s followers:");
                            foreach (string user in followers)
                            {
                                Console.WriteLine(user);
                            }
                            Console.WriteLine("=========");
                        }
                    }
                    // Console.Read();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error! {0}", e.ToString());
            }
            finally
            {
                Console.WriteLine("Finish");
            }
        }
    }
}
